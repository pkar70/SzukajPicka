Imports System.IO

Class MainWindow

    Private mPicList As List(Of JedenPicek)
    Private mFolderList As List(Of JedenFolder)
    Private mInitializing As Boolean = True
    Private mOptionList As String() = {"ja", "as", "ta", "ma", "dw", "bs", "ph", "ch", "zb", "kk", "cj", "cz", "nf", "no"}

    Private mRootDir As String = "L:\HomeArchive\FotoVideo\Analogowe\Zdjecia\dWladziu"

    Private Sub ReadDirectory()
        ' wszystko na bVisible =true
        Dim aFiles = IO.Directory.GetFiles(mRootDir, "*.jpg",
               SearchOption.AllDirectories)

        mPicList = New List(Of JedenPicek)

        For Each sFile In aFiles
            Dim oNew As JedenPicek = New JedenPicek
            oNew.bVisible = True
            oNew.sPathName = sFile
            Dim iInd As Integer = sFile.LastIndexOf("\")
            oNew.sLowerName = sFile.Substring(iInd + 1).ToLower
            iInd = sFile.LastIndexOf("\", iInd - 1)
            oNew.sInitDymek = sFile.Substring(iInd + 1)
            oNew.sDymek = oNew.sInitDymek
            oNew.sDirName = oNew.sPathName.ToLower.Replace(mRootDir.ToLower & "\", "")
            mPicList.Add(oNew)
        Next

        MessageBox.Show("wczytalem " & aFiles.GetUpperBound(0) & " obrazkow")

    End Sub

    Private Sub ReadDirList()
        mFolderList = New List(Of JedenFolder)
        Dim aFolders As String() = IO.Directory.GetDirectories(mRootDir)
        For Each sFolder As String In aFolders
            Dim oNew As JedenFolder = New JedenFolder
            Dim iInd As Integer = sFolder.LastIndexOf("\")
            If iInd > 1 Then
                oNew.sDirName = sFolder.Substring(iInd + 1).ToLower
                oNew.bChecked = True
                mFolderList.Add(oNew)
            End If
        Next
    End Sub

    Private Sub GenerateQueryForm()

        Dim oRowDef As RowDefinition

        Dim iCnt As Integer = 1
        For Each sOption As String In mOptionList

            oRowDef = New RowDefinition
            oRowDef.Height = GridLength.Auto
            uiQuery.RowDefinitions.Add(oRowDef)

            Dim oCBTAK As CheckBox = New CheckBox
            oCBTAK.Name = "ui" & sOption.ToUpper & "Tak"
            oCBTAK.MinWidth = 30
            oCBTAK.Width = 30
            AddHandler oCBTAK.Checked, AddressOf uiPoliczPasujace
            AddHandler oCBTAK.Unchecked, AddressOf uiPoliczPasujace

            Grid.SetRow(oCBTAK, iCnt)
            Grid.SetColumn(oCBTAK, 0)
            uiQuery.Children.Add(oCBTAK)

            Dim oCBNie As CheckBox = New CheckBox
            oCBNie.Name = "ui" & sOption.ToUpper & "Nie"
            oCBNie.MinWidth = 30
            oCBNie.Width = 30
            AddHandler oCBNie.Checked, AddressOf uiPoliczPasujace
            AddHandler oCBNie.Unchecked, AddressOf uiPoliczPasujace

            Grid.SetRow(oCBNie, iCnt)
            Grid.SetColumn(oCBNie, 2)
            If sOption = "nf" Then oCBNie.IsChecked = True
            uiQuery.Children.Add(oCBNie)

            Dim oTxtBk As TextBlock = New TextBlock
            oTxtBk.Text = sOption.ToUpper
            Grid.SetRow(oTxtBk, iCnt)
            Grid.SetColumn(oTxtBk, 1)
            uiQuery.Children.Add(oTxtBk)

            iCnt += 1
            If iCnt > 19 Then
                MessageBox.Show("Za duzo rzadkow zdefiniowales!")
                Return
            End If
        Next

        ' Grid.Row na pole MASK
        oRowDef = New RowDefinition
        oRowDef.Height = GridLength.Auto
        uiQuery.RowDefinitions.Add(oRowDef)

        Grid.SetRow(uiMaskTak, iCnt)
        Grid.SetColumn(uiMaskTak, 0)

        Grid.SetRow(uiMaskNie, iCnt)
        Grid.SetColumn(uiMaskNie, 2)

        iCnt += 1

        oRowDef = New RowDefinition
        oRowDef.Height = GridLength.Auto
        uiQuery.RowDefinitions.Add(oRowDef)

        Grid.SetRow(uiMaskTak2, iCnt)
        Grid.SetColumn(uiMaskTak2, 0)

        Grid.SetRow(uiMaskNie2, iCnt)
        Grid.SetColumn(uiMaskNie2, 2)

        iCnt += 1

        ' Grid.Row na buttonu
        oRowDef = New RowDefinition
        oRowDef.Height = GridLength.Auto
        uiQuery.RowDefinitions.Add(oRowDef)

        Grid.SetRow(uiRunQuery, iCnt)
        Grid.SetColumn(uiRunQuery, 0)

        Grid.SetRow(uiCount, iCnt)
        Grid.SetColumn(uiCount, 1)

        Grid.SetRow(uiShowPics, iCnt)
        Grid.SetColumn(uiShowPics, 2)


    End Sub

    Private Sub GenerateFolderForm()
        uiFolderList.ItemsSource = mFolderList
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sDir As String

        If Application._mParams IsNot Nothing AndAlso Application._mParams.Args.Length = 1 Then
            sDir = Application._mParams.Args.ElementAt(0)
        Else
            sDir = Directory.GetCurrentDirectory()
        End If

        If Not sDir.ToLower.Contains("vstudio") Then mRootDir = sDir

        ReadDirectory()
        GenerateQueryForm()

        ReadDirList()
        GenerateFolderForm()

        If sDir.ToLower.Contains("skanowanie") Then sDir = "skanowanie"
        sDir = sDir.Replace("L:\HomeArchive", "")

        uiMainWin.Title = sDir
        mInitializing = False

    End Sub

    Private Sub uiOpenQuery_Click(sender As Object, e As RoutedEventArgs)
        uiQuery.Visibility = If(uiQuery.Visibility = Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed)
        uiFolders.Visibility = Visibility.Collapsed
    End Sub

    Private Function PoliczPasujace() As Integer

        Dim iCnt As Integer = 0

        For Each oItem As JedenPicek In mPicList
            oItem.bVisible = False
            iCnt += 1
        Next
        Debug.WriteLine("Wszystkich zdjec: " & iCnt)

        ' mFolders
        iCnt = 0
        For Each oItem As JedenPicek In mPicList
            For Each oFold As JedenFolder In mFolderList
                If oFold.bChecked Then
                    If oItem.sDirName.StartsWith(oFold.sDirName) Then ' jedno i drugie jest ToLower przy wczytywaniu
                        oItem.bVisible = True
                        iCnt += 1
                        Exit For
                    End If
                End If
            Next
        Next
        Debug.WriteLine("zdjec w wybranych katalogach: " & iCnt)

        ' mOptionList
        For Each oUIElement As UIElement In uiQuery.Children
            Dim oCheckBox As CheckBox = TryCast(oUIElement, CheckBox)
            If oCheckBox IsNot Nothing Then
                If oCheckBox.IsChecked Then

                    Dim sName As String = oCheckBox.Name.ToLower
                    Dim sMask As String = "-" & sName.Substring(2, 2)

                    For Each oItem As JedenPicek In mPicList
                        If oItem.bVisible Then
                            If sName.Contains("tak") And (Not oItem.sLowerName.Contains(sMask)) Then oItem.bVisible = False
                            If sName.Contains("nie") And oItem.sLowerName.Contains(sMask) Then oItem.bVisible = False
                        End If
                    Next

                End If
            End If
        Next

        iCnt = 0
        For Each oItem As JedenPicek In mPicList
            If oItem.bVisible Then iCnt += 1
        Next
        Debug.WriteLine("zdjec z ludzmi wybranymi: " & iCnt)


        For Each oItem As JedenPicek In mPicList
            If oItem.bVisible Then
                If uiMaskTak.Text.Length > 1 Then
                    If Not oItem.sLowerName.Contains(uiMaskTak.Text.ToLower) Then oItem.bVisible = False
                End If
                If uiMaskNie.Text.Length > 1 Then
                    If oItem.sLowerName.Contains(uiMaskNie.Text.ToLower) Then oItem.bVisible = False
                End If
                If uiMaskTak2.Text.Length > 1 Then
                    If Not oItem.sLowerName.Contains(uiMaskTak2.Text.ToLower) Then oItem.bVisible = False
                End If
                If uiMaskNie2.Text.Length > 1 Then
                    If oItem.sLowerName.Contains(uiMaskNie2.Text.ToLower) Then oItem.bVisible = False
                End If
            End If
        Next

        iCnt = 0
        For Each oItem As JedenPicek In mPicList
            If oItem.bVisible Then iCnt += 1
        Next
        Debug.WriteLine("zdjec po maskach: " & iCnt)

        uiCount.Text = iCnt
        uiCountFold.Text = iCnt

        Return iCnt
    End Function

    Private Sub uiPoliczPasujace(sender As Object, e As RoutedEventArgs)
        If mInitializing Then Return    ' bo sie wiesza na poczatku
        PoliczPasujace()
    End Sub

    Private Sub uiRunQuery_Click(sender As Object, e As RoutedEventArgs)
        'MessageBox.Show("znalazlem " & iCnt & " takich obrazkow")
        uiCount.Text = PoliczPasujace()
    End Sub

    Private Sub uiShowPics_Click(sender As Object, e As RoutedEventArgs)
        uiQuery.Visibility = Visibility.Collapsed
        uiFolders.Visibility = Visibility.Collapsed

        ' obsluga progressbar
        uiPrgBar.Minimum = 0
        Dim iCnt As Integer = 0
        For Each oItem In mPicList
            If oItem.bVisible Then iCnt += 1
        Next
        uiPrgBar.Maximum = iCnt
        uiPrgBar.Value = 0
        uiPrgBar.Visibility = Visibility.Visible


        Dim iPixeli As Integer = uiGrid.ActualWidth * uiGrid.ActualHeight * 0.8   ' na zaokrąglenia
        Dim iPixPerPic As Integer = iPixeli / iCnt  ' pikseli² na obrazek
        Dim iMaxBok As Integer = Math.Sqrt(iPixPerPic)

        iCnt = 0

        ' wczytaj obrazki ktore są bVisible
        For Each oItem In mPicList
            If oItem.bVisible Then
                If oItem.oImageSrc Is Nothing Then
                    ' https://stackoverflow.com/questions/6430299/bitmapimage-in-wpf-does-lock-file
                    Dim bitmap = New BitmapImage()
                    bitmap.BeginInit()
                    bitmap.UriSource = New Uri(oItem.sPathName)
                    bitmap.CacheOption = BitmapCacheOption.OnLoad
                    bitmap.EndInit()
                    oItem.oImageSrc = bitmap
                    ' oItem.oImageSrc = New BitmapImage(New Uri(oItem.sPathName))
                End If

                If oItem.oImageSrc.Width > oItem.oImageSrc.Height Then
                    oItem.iDuzoscH = iMaxBok * 0.66
                Else
                    oItem.iDuzoscH = iMaxBok
                End If

                Dim iCm As Integer = Math.Max(oItem.oImageSrc.Width, oItem.oImageSrc.Height) ' pikseli dłuższego boku
                ' a guzik: Gets the height of the source bitmap in device-independent units (1/96th inch per unit).
                ' iCm = 2.54 * iCm / oItem.oImageSrc.DpiX ' podzielone przez DPI i z cali na centymetry
                iCm = 2.54 * iCm / 96
                oItem.sDymek = oItem.sInitDymek & vbCrLf & "Dłuższy bok: " & iCm & " cm"

            End If
            iCnt += 1
            uiPrgBar.Value = iCnt
            'uiPrgBar.InvalidateVisual()
            'System.Threading.Thread.Sleep(20)

        Next

        ' uaktualnij 
        uiPicList.ItemsSource = From c In mPicList Where c.bVisible = True

        uiPrgBar.Visibility = Visibility.Collapsed
    End Sub

    'Private Sub uiCloseFiles_Click(sender As Object, e As RoutedEventArgs)
    '    uiPicList.ItemsSource = Nothing

    '    For Each oItem In mPicList
    '        If oItem.oImageSrc IsNot Nothing Then
    '            oItem.oImageSrc = Nothing
    '        End If
    '    Next

    'End Sub

    Private Sub uiComboSize_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles uiComboSize.SelectionChanged
        ' zmiana rozmiaru

        If mPicList Is Nothing Then Return

        Dim iCnt As Integer = 0
        For Each oItem In mPicList
            If oItem.bVisible Then iCnt += 1
        Next

        If iCnt < 1 Then Return

        Dim iPixeli As Integer = uiPicList.ActualWidth * uiPicList.ActualHeight * 0.8   ' na zaokrąglenia
        Dim iPixPerPic As Integer = iPixeli / iCnt  ' pikseli² na obrazek
        Dim iMaxBok As Integer = Math.Sqrt(iPixPerPic)

        Dim sRequest As String = TryCast(uiComboSize.SelectedValue, ComboBoxItem).Content
        If sRequest <> "fit" Then iMaxBok = sRequest

        For Each oItem In mPicList
                If oItem.bVisible Then
                    If oItem.oImageSrc Is Nothing Then
                        oItem.oImageSrc = New BitmapImage(New Uri(oItem.sPathName))
                    End If

                    If oItem.oImageSrc.Width > oItem.oImageSrc.Height Then
                        oItem.iDuzoscH = iMaxBok * 0.66
                    Else
                        oItem.iDuzoscH = iMaxBok
                    End If

                End If

        Next

        ' uaktualnij 
        uiPicList.ItemsSource = From c In mPicList Where c.bVisible = True

    End Sub

    Private Sub uiCopyPath_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As MenuItem = sender
        Dim oPicek As JedenPicek = oItem.DataContext
        Clipboard.SetText(oPicek.sPathName)
    End Sub


    Private Sub uiShowBig_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As MenuItem = sender
        Dim oPicek As JedenPicek = oItem.DataContext

        Dim oImage As Image = New Image
        oImage.Source = oPicek.oImageSrc

        'If oImage.Source.Width > oImage.Source.Height Then
        '    oImage.Width = uiPicList.ActualWidth / 2
        'Else
        '    oImage.Height = uiPicList.ActualHeight / 2
        'End If

        'Dim oButt As Button = New Button
        'oButt.Content = "OK"
        'oButt.HorizontalAlignment = HorizontalAlignment.Center

        Dim oStack As StackPanel = New StackPanel
        oStack.Children.Add(oImage)
        'oStack.Children.Add(oButt)

        Dim oWin As Window = New Window
        oWin.Content = oStack
        oWin.Title = oPicek.sInitDymek
        ' oWin.ShowDialog()
        oWin.Show()

    End Sub

    Private Sub uiDubletPic_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As MenuItem = sender
        Dim oPicek As JedenPicek = oItem.DataContext

        Const sDubletDir As String = "L:\Backup\SkanowanieDokumentow\Tata\TakzeNegatywLubDublet"

        If Not oPicek.sPathName.Contains("SkanowanieDokumentow") Then
            MessageBox.Show("Umiem dublety tylko w Skanowanie Dokumentow")
            Return
        End If

        If Not Directory.Exists(sDubletDir) Then
            MessageBox.Show("Nie widze katalogu na dublety" & vbCrLf & sDubletDir)
            Return
        End If

        Dim iInd As Integer = oPicek.sPathName.LastIndexOf("\")
        Dim sNewName As String = sDubletDir & oPicek.sPathName.Substring(iInd)

        'MessageBox.Show("renaming from " & vbCrLf & oPicek.sPathName & vbCrLf & " to " & sNewName)

        Try
            File.Move(oPicek.sPathName, sNewName)
        Catch ex As Exception
            MessageBox.Show("ERROR renaming" & vbCrLf & "From: " & oPicek.sPathName & vbCrLf & "To: " & sNewName)
            Return
        End Try

        oPicek.bVisible = False   ' ale potem moze usunac z listy, a nie tylko tak...

        ' uaktualnij 
        uiPicList.ItemsSource = From c In mPicList Where c.bVisible = True


    End Sub

    Private Sub uiHidePic_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As MenuItem = sender
        Dim oPicek As JedenPicek = oItem.DataContext
        oPicek.bVisible = False ' ale nie wiem czy to zadziala :)
    End Sub

    Private Sub uiRenamePic_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As MenuItem = sender
        Dim oPicek As JedenPicek = oItem.DataContext

        Dim sName As String = oPicek.sPathName
        Dim iInd As Integer = sName.LastIndexOf("\")
        If iInd < 10 Then
            MessageBox.Show("Cos nie tak z nazwą pliku, nie rozumiem jej?")
            Return
        End If
        sName = sName.Substring(iInd + 1)

        Dim sNewName As String = InputBox("Nowa nazwa:", "SzukajPicka", sName)
        If sNewName.Length < 2 Then Return

        If sName = sNewName Then
            MessageBox.Show("Bez zmiany...")
            Return
        End If

        sName = oPicek.sPathName.Substring(0, iInd + 1) + sNewName

        MessageBox.Show("renaming from " & vbCrLf & oPicek.sPathName & vbCrLf & " to " & sName)

        Try
            File.Move(oPicek.sPathName, sName)
        Catch ex As Exception
            MessageBox.Show("ERROR renaming" & vbCrLf & "From: " & oPicek.sPathName & vbCrLf & "To: " & sName)
            Return
        End Try

        oPicek.sPathName = sName
        oPicek.sLowerName = sName.Substring(iInd + 1).ToLower
        iInd = oPicek.sPathName.LastIndexOf("\", iInd - 1)
        oPicek.sInitDymek = oPicek.sPathName.Substring(iInd + 1)

        Dim iCm As Integer = Math.Max(oPicek.oImageSrc.Width, oPicek.oImageSrc.Height) ' pikseli dłuższego boku
        ' a guzik: Gets the height of the source bitmap in device-independent units (1/96th inch per unit).
        ' iCm = 2.54 * iCm / oItem.oImageSrc.DpiX ' podzielone przez DPI i z cali na centymetry
        iCm = 2.54 * iCm / 96
        oPicek.sDymek = oPicek.sInitDymek & vbCrLf & "Dłuższy bok: " & iCm & " cm"
    End Sub

    Private Sub uiFolders_Click(sender As Object, e As RoutedEventArgs)
        uiFolders.Visibility = If(uiFolders.Visibility = Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed)
        uiQuery.Visibility = Visibility.Collapsed
    End Sub

    Private Sub uiFolderSelected(sender As Object, e As RoutedEventArgs)
        uiCountFold.Text = PoliczPasujace()
    End Sub

    Private Sub uiFolderOnlyThis(sender As Object, e As MouseButtonEventArgs)
        Dim oCB As CheckBox = TryCast(sender, CheckBox)
        If oCB Is Nothing Then Return

        Dim sFolder As String = TryCast(oCB.DataContext, JedenFolder).sDirName

        For Each oItem As JedenFolder In mFolderList
            If oItem.sDirName = sFolder Then
                oItem.bChecked = True
            Else
                oItem.bChecked = False
            End If
        Next

    End Sub
End Class

Class JedenPicek
    Public Property sLowerName As String
    Public Property sPathName As String
    Public Property sInitDymek As String
    Public Property bVisible As Boolean
    Public Property sDymek As String
    Public Property oImageSrc As BitmapImage = Nothing
    Public Property iDuzoscH As Integer
    Public Property sDirName As String  ' w ramach mRootDir
End Class

Class JedenFolder
    Public Property sDirName As String
    Public Property bChecked As Boolean
End Class


