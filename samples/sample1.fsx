#I @"../packages/Suave/lib/net40"
#r "Suave.dll"
#I @"../packages/Suave.RouteTypeProvider/lib"
#r "Suave.RouteTypeProvider.dll"

open Suave
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RouteTypeProvider

let config =
   { defaultConfig
       with bindings =
             [ HttpBinding.mk HTTP IPAddress.Loopback 8000us ] }

module Routes =
  type FindUserById = routeTemplate<"/findUser/{id:int}">
  type AdditionRoute = routeTemplate<"/add2/{value1:int}/{value2:int}">
  type SayBonjour = routeTemplate<template="/bonjour/{Name:string}/{FirstName:string}/{Age:int}",
                                  description="Say hello in french">

let app =
 choose // combines 'post' and 'get' WebPart
   [ GET >=> choose // combines both WebParts
       [ path "/hello" >=> OK "Hello GET"
         path "/goodbye" >=> OK "Good bye GET"

         Routes.SayBonjour.Returns(fun m -> OK <| sprintf "bonjour %s" m.FirstName)

         Routes.FindUserById.Returns(fun m -> OK <| sprintf "id is: %A" m.id)
         Routes.AdditionRoute.Returns(fun m -> OK <| (m.value1 + m.value2).ToString())

//         pathScan "/add/%d/%d" (
//              fun (n1,n2) ->
//                OK <| sprintf "%d" (n1 + n2)
//            )
          ]
     POST >=> choose // combines both WebParts
       [ path "/hello" >=> OK "Hello POST"
         path "/goodbye" >=> OK "Good bye POST" ] ]


// passing the final WebPart to the HTTP server
startWebServer config app
