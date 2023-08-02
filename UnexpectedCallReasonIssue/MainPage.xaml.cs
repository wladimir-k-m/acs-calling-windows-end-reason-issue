using Azure.Communication.Calling.WindowsClient;
using System.Diagnostics;

namespace UnexpectedCallReasonIssue;

public partial class MainPage : ContentPage
{
    private IncomingCall _incomingCall;

	public MainPage()
	{
		InitializeComponent();
	}

    private void OnIncomingCall(object sender, IncomingCallReceivedEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InfoLabel.Text = $"Incoming Call...";
        });
        _incomingCall = args.IncomingCall;
        args.IncomingCall.CallEnded += OnCallEnded;
    }

    private void OnCallEnded(object sender, PropertyChangedEventArgs e)
    {
        _incomingCall.CallEnded -= OnCallEnded;
        Trace.WriteLine($"Call Ended Reason:\nCode: {_incomingCall.CallEndReason.Code}\nSubcode: {_incomingCall.CallEndReason.Subcode}");

        MainThread.BeginInvokeOnMainThread(() =>
        {
            InfoLabel.Text = $"Call Ended Reason:\nCode: {_incomingCall.CallEndReason.Code}\nSubcode: {_incomingCall.CallEndReason.Subcode}";
        });
    }

    private async void OnLoginClicked(object sender, EventArgs e)
	{
        try
        {
            await TeamsCallListener.LoginAndListen();
            TeamsCallListener.CallAgent.IncomingCallReceived += OnIncomingCall;
            InfoLabel.Text = "Logged in";
        }
        catch (Exception ex)
        {
            InfoLabel.Text = $"Error: {ex.Message}";
        }
	}
}

