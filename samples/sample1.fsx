#I @"../packages/Suave/lib/net40"
#r "Suave.dll"

open Suave
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.Successful

let config =
   { defaultConfig
       with bindings =
             [ HttpBinding.mk HTTP IPAddress.Loopback 8000us ] }

let app =
 choose // combines 'post' and 'get' WebPart
   [ GET >=> choose // combines both WebParts
       [ path "/hello" >=> OK "Hello GET"
         path "/goodbye" >=> OK "Good bye GET"
         pathScan "/add/%d/%d" (
              fun (n1,n2) ->
                OK <| sprintf "%d" (n1 + n2)
            )
          ]
     POST >=> choose // combines both WebParts
       [ path "/hello" >=> OK "Hello POST"
         path "/goodbye" >=> OK "Good bye POST" ] ]



// passing the final WebPart to the HTTP server
startWebServer config app
