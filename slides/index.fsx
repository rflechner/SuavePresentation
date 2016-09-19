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


## How to start we Suave ?

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
 - Computation expression ( Monoid ?? )

---




*)
