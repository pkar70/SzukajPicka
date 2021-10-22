Class Application

    Public Shared _mParams As StartupEventArgs

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)
        _mParams = e
    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

End Class
