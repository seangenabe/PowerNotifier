Imports Microsoft.Toolkit.Uwp.Notifications
Imports Tasks
Imports Windows.ApplicationModel.Background
Imports Windows.UI.Notifications

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
  Inherits Page

  Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
    Await UnregisterAll()

    Dim powerTrigger As New SystemTrigger(SystemTriggerType.PowerStateChange, False)
    Await Register(powerTrigger, NotifierTask.TaskString, NotifierTask.EntryPointString)

    ShowSetupDoneNotification()

    mainProgressRing.IsActive = False

#If DEBUG Then
    debugPanel.Visibility = Visibility.Visible
#Else
    App.Current.Exit()
#End If
  End Sub

  Private Sub ShowSetupDoneNotification()
    Dim binding As New ToastBindingGeneric()
    binding.Children.Add(New AdaptiveText() With {.Text = "Ready to send power state notifications."})
    Dim visual As New ToastVisual() With {
      .BindingGeneric = binding
    }

    Dim content As New ToastContent() With {
      .Visual = visual,
      .ActivationType = ToastActivationType.Background
    }

    Dim toast As New ToastNotification(content.GetXml())

    ToastNotificationManager.CreateToastNotifier().Show(toast)
  End Sub

  Private Async Sub debugButton_Click(sender As Object, e As RoutedEventArgs) Handles debugButton.Click
    If Await RequestBackgroundAccess() Then
      Dim a = BackgroundTaskRegistration.AllTasks.ToList()
      Dim b = ToastNotificationManager.History.GetHistory().ToList()
    End If
  End Sub

End Class
