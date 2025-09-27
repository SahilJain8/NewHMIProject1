#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
using UAManagedCore;
using FTOptix.NetLogic;
using FTOptix.CommunicationDriver;
using FTOptix.EventLogger;
#endregion

public class RuntimeNetLogic2 : BaseNetLogic
{
    private CancellationTokenSource _cts;
    private Task _worker;
    private IUAVariable _velocity;
    private IUAVariable _acc;
    private IUAVariable _isActive;   // <-- keep as a variable
    private bool _led;
    // <-- field that mirrors _isActive.Value
    private IUAVariable temp;

    public override void Start()
    {
        _velocity = LogicObject.GetVariable("velocity");
        _acc = LogicObject.GetVariable("acc");
        _isActive = LogicObject.GetVariable("isactive");
        temp = LogicObject.GetVariable("Variable2");

        if (_velocity == null) throw new InvalidOperationException("Variable 'velocity' not found.");
        if (_acc == null) throw new InvalidOperationException("Variable 'acc' not found.");
        if (_isActive == null) throw new InvalidOperationException("Variable 'isactive' not found.");

        // read once (use Convert.ToBoolean because .Value is a Variant)
        _led = (Boolean) _isActive.Value ;

        // if you want to start based on current state:
        if (_led) StartRandomizer();
        
        
    }

    public override void Stop()
    {
        StopRandomizer();
    }

    [ExportMethod]
    public void StopRandomizerAndZero()
    {
        StopRandomizer();
        _velocity.Value = 0f;
        _acc.Value = 0f;
    }

    [ExportMethod]
    public void StartRandomizer()
    {
        // Always re-read the LED state at call time
        _isActive = LogicObject.GetVariable("isactive");
        _led = (Boolean)_isActive.Value;
        Log.Info($"LED active: {_led}");

        if (!_led) return;

        if (_cts != null)
            StopRandomizer();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        var rand = new Random();
        const float min = 10f, max = 100f;

        _worker = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                _velocity.Value = rand.NextSingle() * (max - min) + min;
                _acc.Value = rand.NextSingle() * (max - min) + min;
                await Task.Delay(300, token).ConfigureAwait(false);
            }
        }, token);
    }

    private void StopRandomizer()
    {
        var cts = _cts;
        if (cts == null) return;

        try
        {
            cts.Cancel();
            _worker?.Wait();
        }
        catch (AggregateException) { }
        finally
        {
            cts.Dispose();
            _cts = null;
            _worker = null;
        }
    }

   
}
