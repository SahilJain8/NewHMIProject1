#region Using directives
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.ODBCStore;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.Store;
using FTOptix.UI;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UAManagedCore;
using FTOptix.CommunicationDriver;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class RuntimeNetLogic3 : BaseNetLogic
{
    private Button button1;
    private IUAVariable pressedVar;

    public override void Start()
    {
        button1 = (Button)Owner; // NetLogic attached directly to the Button
        pressedVar = Project.Current.GetVariable("Model/ButtonPressed");

        if (pressedVar != null)
            pressedVar.VariableChange += PressedVarChanged;

        // Default color
        button1.BackgroundColor = Colors.Red;
    }

    private void PressedVarChanged(IUAVariable variable, VariableChangeEventArgs e)
    {
        bool isPressed = e.NewValue.Value; // NewValue is a UAValue
        button1.BackgroundColor = isPressed ? Colors.Green : Colors.Red;
    }

    public override void Stop()
    {
        if (pressedVar != null)
            pressedVar.VariableChange -= PressedVarChanged;
    }
}
