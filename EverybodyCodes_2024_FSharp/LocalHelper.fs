module LocalHelper

///////////////////////////////////////////////////////////////////////////////////
/// Returns duration of the execution of the function
let duration f = 
    let timer = new System.Diagnostics.Stopwatch()
    timer.Start()
    let returnValue = f()
    //printfn "Elapsed Time: %i" timer.ElapsedMilliseconds
    (returnValue, timer.ElapsedTicks)
///////////////////////////////////////////////////////////////////////////////////   