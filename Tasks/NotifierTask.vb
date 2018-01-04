Imports Microsoft.Toolkit.Uwp.Notifications
Imports Windows.ApplicationModel.Background
Imports Windows.System.Power
Imports Windows.UI.Notifications

Public NotInheritable Class NotifierTask
  Implements IBackgroundTask

  Public Shared ReadOnly Property TaskString As String = NameOf(NotifierTask)
  Public Shared ReadOnly Property EntryPointString As String = GetType(NotifierTask).FullName

  Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
    Dim status = PowerManager.PowerSupplyStatus

    Dim existingToast = (
      From toast In ToastNotificationManager.History.GetHistory()
      Where toast.Data IsNot Nothing AndAlso
        status = [Enum].Parse(Of PowerSupplyStatus)(toast.Data.Values.ItemOrDefault("PowerSupplyStatus"))
      Select toast).FirstOrDefault()

    ' Check if we have already notified with this status.
    ' (SystemTrigger fires multiple times.)
    ' (Same tag clears history.)
    If existingToast IsNot Nothing Then
      Return
    End If

    ToastNotificationManager.CreateToastNotifier().Show(CreatePowerNotification(status))
  End Sub

  Private Function CreatePowerNotification(status As PowerSupplyStatus) As ToastNotification
    Dim binding As New ToastBindingGeneric()
    binding.Children.Add(New AdaptiveText() With {.Text = GetPowerStatus(status)})
    Dim visual As New ToastVisual() With {
      .BindingGeneric = binding
    }

    Dim content As New ToastContent() With {
      .Visual = visual,
      .ActivationType = ToastActivationType.Background
    }

    Dim toast As New ToastNotification(content.GetXml())
    Dim data = New NotificationData()
    data.Values.Item("PowerSupplyStatus") = CStr(status)
    toast.Data = data
    toast.Tag = "PowerNotifier.NotifierTask"

    Return toast
  End Function

  Private Function GetPowerStatus(status As PowerSupplyStatus) As String
    Select Case PowerManager.PowerSupplyStatus
      Case PowerSupplyStatus.NotPresent
        Return "The device is not plugged in."
      Case PowerSupplyStatus.Inadequate
        Return "The device is plugged in, but not charging."
      Case PowerSupplyStatus.Adequate
        Return "The device is plugged in."
    End Select
    Debug.Assert(False, "Unknown PowerSupplyStatus value.")
    Throw New InvalidOperationException("Unknown PowerSupplyStatus value.")
  End Function

End Class
