(**
- title : Have fun with Suave
- description : Have fun with Suave !
- author : Romain Flechner
- theme : league
- transition : default

***

*)

(*** hide ***)
#I @"../packages/Suave/lib/net40"
#r "Suave.dll"

#I @"../packages/Suave.Swagger/lib"
#r "Suave.Swagger.dll"

open System
open Suave
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.Successful

(**
### Have fun with Suave and FSharp

<img src="images/fsharp128.png" height="100">
<img src="images/lovefs.png" height="50">

Made for fsharpparis

<img src="images/eiffel.svg" width="100">

***

## What is Suave?

- [Suave](https://suave.io/) is a FSharp lightweight web server principally used to develop REST APIs

***

- id : discover Suave


## How to start with Suave ?

---

### Install Suave NuGet

___ With Paket ___

    [lang=shell]
    paket add nuget Suave -i

___ Or with NuGet ___

    [lang=powershell]
    Install-Package Suave

---

### Write a tiny peace of code

    [lang=fsharp]
    open Suave
    open System.Net

    let config =
      { defaultConfig
          with bindings =
                [ HttpBinding.mk HTTP IPAddress.Loopback 8000us ] }
    // startWebServer config (Successful.OK "Hello World!")

The URL http://localhost:8000/   should return "Hello World!" as content

---
### Enjoy :)

![screen1](images/screen1.png)

---

### Routing requests

    [lang=fsharp]
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful

    let app =
      choose // combines 'post' and 'get' WebPart
        [ GET >=> choose // combines both WebParts
            [ path "/hello" >=> OK "Hello GET"
              path "/goodbye" >=> OK "Good bye GET" ]
          POST >=> choose // combines both WebParts
            [ path "/hello" >=> OK "Hello POST"
              path "/goodbye" >=> OK "Good bye POST" ] ]
    // passing the final WebPart to the HTTP server
    // startWebServer config app

---

### Trying ...

![screen2](images/screen2.png)

phew! it works :)

***
- id : typed routes

# Typed routes

---

## The pathScan function

*)
let app =
  GET >=> choose // combines both WebParts
       [ path "/hello" >=> OK "Hello GET"
         pathScan "/add/%d/%d" (
              fun (n1,n2) ->
                OK <| sprintf "%d" (n1 + n2)
            ) ]
(**

The pathScan function uses the url template to create a strongly typed function.

So n1 and n2 are integers :)

---

## Nothing is magic

<img src="images/magic_hat.svg" width="300">

---

### The power of PrintfFormat

*)

let analyse (pf : PrintfFormat<_,_,_,_,'t>) =
  match typeof<'t> with
  | ty when ty.IsPrimitive -> sprintf "Primitive type of %s " ty.Name
  | ty ->
    let names =
      ty.GetGenericArguments()
      |> Seq.map (fun pt -> pt.Name)
      |> Seq.toList
    sprintf "Generic of %A" names

let log1 = analyse "/stringify/%d"
let log2 = analyse "/add/%d/%d/%s"

(** log1 and log2 are:  *)
(*** include-value: log1 ***)
(*** include-value: log2 ***)

(**
---
###

So we can write something like:

*)
let pathTemplate (pf : PrintfFormat<_,_,_,_,'t>) (callback:'t -> unit)=
  failwith "not implemented"

pathTemplate "/add/%d/%d" (fun (n1:int,n2:int) -> failwith "not implemented")

(**

***
# Extending Suave

***

## How to have named and typed route params ?

The pathScan function doesn't name parameters.

So they can be confused when they are many.

---

### A solution could be a type provider

https://rflechner.github.io/Suave.RouteTypeProvider/

![screen3](images/screen3.gif)

---

### Benefits

- Strongly typed routes
- Better code organization ( routes declared in types )
- Autocompletion when typing routes handlers ( fun )

***

## Testing and documenting

---

### Swagger is great !

http://petstore.swagger.io/#/pet

<img src="images/swagger1.png" width="500">

---

### What is it ?

- Representation of your RESTful API
- Interactive generated documentation
- client SDK generation
- discoverability (like WSDL)

---

### Swagger for Suave ?

---

#### Problem:
 - Routes are pure fonctions and NOT discoverables

#### Solution :
 - Wrapping functions to keep informations
 - Computation expression

---

### How to swagger your Sauve project ?

Go to

- https://rflechner.github.io/Suave.Swagger/

- Or https://www.nuget.org/packages/Suave.Swagger/

---

### Getting started

---

#### Tiny example

We want to create an API returning current time.

##### In a " classic " mode

*)

let now1 : WebPart =
  fun (x : HttpContext) ->
    async {
      return! OK (DateTime.Now.ToString()) x
    }
//[<EntryPoint>]
let main argv =
  let time1 = GET >=> path "/time1" >=> now1
  //startWebServer defaultConfig time1
  0 // return an integer exit code

(**

---

##### With the verbose DSL

*)

open Suave.Swagger
open Rest
open FunnyDsl
open Swagger

let now2 : WebPart =
  fun (x : HttpContext) ->
    async {
      return! MODEL DateTime.Now x
    }
let api1 =
  swagger {
    for route in getting (simpleUrl "/time" |> thenReturns now2) do
      yield description Of route is "What time is it ?"
  }
//startWebServer defaultConfig api1.App

(**
---
##### Result

Go to http://localhost:8083/swagger/v2/ui/index.html

<img src="images/swagger2.gif" width="600">

***

# Concrete and unusual using case

---

We want to create an API sending and receiving SMS for a personal and domotic projects.

<img src="images/robot_and_phones.jpg" width="600">

The robot must send SMS using old smartphones.

---

So I created a REST API hosted under Android.

The project is here: https://github.com/rflechner/SmsServer

<img src="images/Screenshot_20160920-230341.png" width="200">

( Wow really beautiful UI ^^ )

---

## Xamarin

Suave exists in the Xamarin components store

![xamarin studio](images/xamarin_studio1.png)

---

## Project structure

- The UI is in a PCL project using Xamarin.Forms
- The Android project is using the PCL to bind UI elements with their behaviors
- Swagger DSL source code is temporary copied in the Android project.

---

## Focus on the API code

#### Models

*)

[<CLIMutable>]
type SmsModel = {
  Id:int
  Address:string
  Body:string
  Date:DateTime
}
and [<CLIMutable>] SendSmsModel = {
  Destination:string
  Body:string
}
and [<CLIMutable>] SendSmsResult = {
  Success:bool
  Sms:SendSmsModel
}

(**

---

#### To know about Android

The most of phone's data we need are stored in system's SQlite databases.

So this snippet will help us to get them quickly.

    [lang=fsharp]
    let readAllRows (context:Context) uri =
      let cr = context.ContentResolver
      use c = cr.Query(Android.Net.Uri.Parse uri, null, null, null, null)
      let names = c.GetColumnNames()
      c.MoveToFirst() |> ignore
      seq {
        for _ in 0 .. c.Count-1 do
          let row = names
            |> Seq.map (
                fun name ->
                      name, c.GetString(c.GetColumnIndex name))
            |> dict
          yield row
          c.MoveToNext() |> ignore
        c.Close()
      } |> Seq.toArray

---

#### Then most useful functions will be

    [lang=fsharp]
    let getContacts context =
      let uri = Android.Provider.ContactsContract
                  .CommonDataKinds.Phone.ContentUri.ToString()
      readAllRows context uri
    let listSentSms context skip limit =
        match skip with
        | Some n -> listSms context "content://sms/sent" n limit
        | None -> listSms context "content://sms/sent" 0 limit
    let listInboxSms context skip limit =
      match skip with
      | Some n -> listSms context "content://sms/inbox" n limit
      | None -> listSms context "content://sms/inbox" 0 limit
    // etc ...

---

#### Now writing API

    [lang=fsharp]

    let api =
      swagger {
        for route in getting (simpleUrl "/contacts"
                              |> thenReturns contacts) do
          yield description Of route is "contacts"

        for route in getting (simpleUrl "/sms/sent"
                               |> thenReturns (sentSmsWp context)) do
          // this is the raw description of the REST method
          yield description Of route is "Get last 20 sent SMS"
          // we can provide a type per response code
          yield route
                |> addResponse 200 "The found messages"
                    (Some typeof<SmsModel>)
      }

***

# DEMO

*)
