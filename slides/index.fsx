(**
- title : Have fun with Suave
- description : Have fun with Suave !
- author : Romain Flechner
- theme : league
- transition : default

***

### Have fun with Suave and FSharp

<img src="images/fsharp128.png" height="100">
<img src="images/lovefs.png" height="50">

Made for fsharpparis

<img src="images/eiffel.svg" width="100">

***

## What is Suave?

- [Suave](https://suave.io/) is a FSharp lightweight web server principally used to develop REST APIs

***

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
    startWebServer config (Successful.OK "Hello World!")

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
    startWebServer config app

---

### Trying ...

![screen2](images/screen2.png)

phew! it works :)

***

# Typed routes


*)
