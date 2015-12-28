module bah
//open System
//open System.Threading.Tasks
//open System.ServiceModel
//open System.ServiceModel.Description
//open Microsoft.FSharp.Linq
//open Microsoft.FSharp.Data.TypeProviders
//open System.Runtime.Serialization
//open System.IdentityModel
//
//let tryExecute (m : unit -> DateTime) s = async{
//    try
//        let now = m()
//        Console.WriteLine(s + "\t" + now.ToString("HH:mm:ss") + "\tSUCCESS")
//    with
//        | ex -> 
//            Console.WriteLine(s + "\tFAIL" + Environment.NewLine + ex.ToString())
//    }
//
//let binding isHttps : Channels.Binding =
//    match isHttps with
//    | true -> 
//        let b = new BasicHttpsBinding()
//        b.Security.Mode <- BasicHttpsSecurityMode.Transport
//        b.Security.Transport.ClientCredentialType <- HttpClientCredentialType.Windows
//        b :> Channels.Binding
//    | false ->        
//        let b = new BasicHttpBinding()
//        b :> Channels.Binding
//
//let channel<'serviceType> (address : string) =
//    if address.Length < 6 then failwith ("Address: " + address + " is not valid")
//    let isHttps = if address.[0..4] = "https" then true else false
//    let factory = new ChannelFactory<'serviceType>()
//    factory.Endpoint.Address <- new EndpointAddress(address)
//    factory.Endpoint.Binding <- binding isHttps
//    let channel = factory.CreateChannel()
//    channel
//        
//[<EntryPoint>]
//let main argv = 
//    let channel1 = (channel<Oot.MT_1.ITimeService> "http://localhost:4710/MT_1_v1.svc")
//    [tryExecute (channel<Oot.MT_1.ITimeService> "http://localhost:4710/MT_1_v1.svc").GetTime "MT_1_v1.GetTime() http"; 
//     tryExecute (channel<Oot.MT_1.ITimeService> "https://localhost:4711/MT_1_v1.svc").GetTime "MT_1_v1.GetTime() https"] 
//    |> Async.Parallel
//    |> Async.RunSynchronously
//    |> ignore
//    
//    0