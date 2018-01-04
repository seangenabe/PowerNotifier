Imports Windows.ApplicationModel.Background

Public Module Tasks

  Public Function UnregisterAll() As IAsyncAction
    Return Unregister(NotifierTask.TaskString)
  End Function

  Public Function RequestBackgroundAccess() As IAsyncOperation(Of Boolean)
    Return AsyncInfo.Run(Of Boolean)(
      Async Function() As Task(Of Boolean)
        Dim backgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()
        Return backgroundAccessStatus = BackgroundAccessStatus.AllowedSubjectToSystemPolicy OrElse
          backgroundAccessStatus = BackgroundAccessStatus.AlwaysAllowed
      End Function)
  End Function

  Public Function Unregister(taskString As String) As IAsyncAction
    Return AsyncInfo.Run(
      Async Function() As Task
        If Await RequestBackgroundAccess() Then
          For Each task In BackgroundTaskRegistration.AllTasks
            If task.Value.Name = taskString Then
              task.Value.Unregister(True)
            End If
          Next
        End If
      End Function)
  End Function

  Public Function Register(
      trigger As IBackgroundTrigger,
      taskString As String,
      entryPoint As String
  ) As IAsyncOperation(Of BackgroundTaskRegistration)
    Return AsyncInfo.Run(
      Async Function(cancel) As Task(Of BackgroundTaskRegistration)
        ' Register new background task
        If Not Await RequestBackgroundAccess() Then
          Return Nothing
        End If
        Dim taskBuilder As New BackgroundTaskBuilder With {
              .Name = taskString,
              .TaskEntryPoint = entryPoint
            }
        taskBuilder.SetTrigger(trigger)
        Return taskBuilder.Register()
      End Function)
  End Function

End Module
