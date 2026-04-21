Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging.ImageProcessing.Document
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Utils
Imports Atalasoft.Utils.Geometry
Imports WinDemoHelperMethods.WinDemoHelperMethods

Namespace AdvancedDocClean
    ''' <summary>
    ''' Summary description for Form1.
    ''' </summary>
    Public Class Form1 : Inherits System.Windows.Forms.Form

#Region "Processing Commands"

        Private _autoBinarize As Atalasoft.Imaging.ImageProcessing.Document.BinarizeCommand
        Private _autoBorderCrop As Atalasoft.Imaging.ImageProcessing.Document.AutoBorderCropCommand
        Private _autoInverseTextCorrection As Atalasoft.Imaging.ImageProcessing.Document.AutoInvertTextCommand
        Private _autoNegate As Atalasoft.Imaging.ImageProcessing.Document.AutoNegateCommand
        Private _blankPageDetection As Atalasoft.Imaging.ImageProcessing.Document.BlankPageDetectionCommand
        Private _blobRemoval As Atalasoft.Imaging.ImageProcessing.Document.BlobRemovalCommand
        Private _borderRemoval As Atalasoft.Imaging.ImageProcessing.Document.AdvancedBorderRemovalCommand
        Private _holePunchRemoval As Atalasoft.Imaging.ImageProcessing.Document.HolePunchRemovalCommand
        Private _lineRemoval As Atalasoft.Imaging.ImageProcessing.Document.LineRemovalCommand
        Private _speckRemoval As Atalasoft.Imaging.ImageProcessing.Document.SpeckRemovalCommand
        Private _marginCrop As Atalasoft.Imaging.ImageProcessing.Document.MarginCropCommand
        Private _despeckle As Atalasoft.Imaging.ImageProcessing.Document.DocumentDespeckleCommand
        Private _deskew As Atalasoft.Imaging.ImageProcessing.Document.AutoDeskewCommand
        Private _halftoneremoval As Atalasoft.Imaging.ImageProcessing.Document.HalftoneRemovalCommand

#End Region

        Private _selectionMessageShown As Boolean
        Private _watchSelection As Boolean
        Private _validLicense As Boolean

        Private mainMenu1 As System.Windows.Forms.MainMenu
        Private menuItem4 As System.Windows.Forms.MenuItem
        Private panel1 As System.Windows.Forms.Panel
        Private panel2 As System.Windows.Forms.Panel
        Private WithEvents listCommands As System.Windows.Forms.ListBox
        Private WithEvents cboFilter As System.Windows.Forms.ComboBox
        Private label1 As System.Windows.Forms.Label
        Private WithEvents panelViewers As System.Windows.Forms.Panel
        Private WithEvents wvOriginal As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Private WithEvents wvProcessed As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Private menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuFileOpen As System.Windows.Forms.MenuItem
        Private WithEvents menuFileSave As System.Windows.Forms.MenuItem
        Private WithEvents menuExit As System.Windows.Forms.MenuItem
        Private menuProcess As System.Windows.Forms.MenuItem
        Private WithEvents menuProcessImage As System.Windows.Forms.MenuItem
        Private WithEvents menuProcessSelection As System.Windows.Forms.MenuItem
        Private statusBar1 As System.Windows.Forms.StatusBar
        Private panelGrid As System.Windows.Forms.Panel
        Private WithEvents btnProcess As System.Windows.Forms.Button
        Private propertyGrid1 As System.Windows.Forms.PropertyGrid
        Private statusFilename As System.Windows.Forms.StatusBarPanel
        Private statusImageSize As System.Windows.Forms.StatusBarPanel
        Private statusDpi As System.Windows.Forms.StatusBarPanel
        Private statusPixelFormat As System.Windows.Forms.StatusBarPanel
        Private statusPosition As System.Windows.Forms.StatusBarPanel
        Private statusTime As System.Windows.Forms.StatusBarPanel
        Private statusInfo As System.Windows.Forms.StatusBarPanel
        Private menuHelp As System.Windows.Forms.MenuItem
        Private WithEvents menuHelpAbout As System.Windows.Forms.MenuItem
        Private openFileDialog1 As System.Windows.Forms.OpenFileDialog
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.Container = Nothing

        Public Sub New()
            Try
                CheckLicenseFile()

                If Me._validLicense Then
                    '
                    ' Required for Windows Form Designer support
                    '
                    InitializeComponent()
                    HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders)
                    Me.openFileDialog1.Filter = HelperMethods.CreateDialogFilter(True)

                    'Me.cboFilter.SelectedIndex = 0

                    ' This should point to the "DotImage 4.0\Images\Documents" folder.
                    Me.openFileDialog1.FileName = System.IO.Path.GetFullPath("..\..\Images\Documents\HolePunchBorderRemovalDeskewSample.tif")
                    If (Not System.IO.File.Exists(Me.openFileDialog1.FileName)) Then
                        Me.openFileDialog1.FileName = System.IO.Path.GetFullPath("..\..\..\..\..\Images\Documents\HolePunchBorderRemovalDeskewSample.tif")
                        If (Not System.IO.File.Exists(Me.openFileDialog1.FileName)) Then
                            Me.openFileDialog1.FileName = ""
                        End If
                    End If
                End If
            Catch e As System.Exception
                Throw New System.Exception("ADC demo constructor", e)
            End Try
        End Sub

#Region "Check for license code"

        Private Sub CheckLicenseFile()
            ' Make sure a license for DotImage and Advanced DocClean exist.
            Try
                Dim img As AtalaImage = New AtalaImage
                img.Dispose()
            Catch e1 As Atalasoft.Imaging.AtalasoftLicenseException
                LicenseCheckFailure("This demo requires a DotImage Document Imaging license and an Advanced DocClean license.")
                Return
            End Try

            If AtalaImage.Edition <> LicenseEdition.Document Then
                LicenseCheckFailure("This demo requires a Document Imaging License." & Constants.vbCrLf & "Your current license is for '" & AtalaImage.Edition.ToString() & "'.")
                Return
            End If

            Try
                CreateCommands()
                Me._validLicense = True
            Catch e1 As Atalasoft.Imaging.AtalasoftLicenseException
                LicenseCheckFailure("This demo requires an Advanced DocClean license.")
            End Try
        End Sub

        Private Sub LicenseCheckFailure(ByVal message As String)
            AddHandler Load, AddressOf Form1_Load
            If MessageBox.Show(Me, message & Constants.vbCrLf & Constants.vbCrLf & "Would you like to request an evaluation license?", "License Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then
                ' Locate the activation utility.
                Dim path As String = ""
                Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\Atalasoft\dotImage\5.0")
                If Not key Is Nothing Then
                    path = Convert.ToString(key.GetValue("AssemblyBasePath"))
                    If Not path Is Nothing AndAlso path.Length > 5 Then
                        path = path.Substring(0, path.Length - 3) & "AtalasoftToolkitActivation.exe"
                    Else
                        path = System.IO.Path.GetFullPath("..\..\..\..\..\AtalasoftToolkitActivation.exe")
                    End If

                    key.Close()
                End If

                If File.Exists(path) Then
                    System.Diagnostics.Process.Start(path)
                Else
                    MessageBox.Show(Me, "We were unable to location the DotImage activation utility." & Constants.vbCrLf & "Please run it from the Start menu shortcut.", "File Not Found")
                End If
            End If
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs)
            If (Not Me._validLicense) Then
                Application.Exit()
            End If
        End Sub

#End Region

        Private Sub CreateCommands()
            _autoBinarize = New BinarizeCommand
            _autoBorderCrop = New AutoBorderCropCommand
            _autoInverseTextCorrection = New AutoInvertTextCommand
            _autoNegate = New AutoNegateCommand
            _blankPageDetection = New BlankPageDetectionCommand
            _blobRemoval = New BlobRemovalCommand
            _borderRemoval = New AdvancedBorderRemovalCommand
            _holePunchRemoval = New HolePunchRemovalCommand
            _lineRemoval = New LineRemovalCommand
            _speckRemoval = New SpeckRemovalCommand
            _marginCrop = New MarginCropCommand
            _despeckle = New DocumentDespeckleCommand
            _deskew = New AutoDeskewCommand
            _halftoneremoval = New HalftoneRemovalCommand
        End Sub

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.mainMenu1 = New System.Windows.Forms.MainMenu
            Me.menuFile = New System.Windows.Forms.MenuItem
            Me.menuFileOpen = New System.Windows.Forms.MenuItem
            Me.menuFileSave = New System.Windows.Forms.MenuItem
            Me.menuItem4 = New System.Windows.Forms.MenuItem
            Me.menuExit = New System.Windows.Forms.MenuItem
            Me.menuProcess = New System.Windows.Forms.MenuItem
            Me.menuProcessImage = New System.Windows.Forms.MenuItem
            Me.menuProcessSelection = New System.Windows.Forms.MenuItem
            Me.menuHelp = New System.Windows.Forms.MenuItem
            Me.menuHelpAbout = New System.Windows.Forms.MenuItem
            Me.panel1 = New System.Windows.Forms.Panel
            Me.panelGrid = New System.Windows.Forms.Panel
            Me.propertyGrid1 = New System.Windows.Forms.PropertyGrid
            Me.btnProcess = New System.Windows.Forms.Button
            Me.panel2 = New System.Windows.Forms.Panel
            Me.listCommands = New System.Windows.Forms.ListBox
            Me.cboFilter = New System.Windows.Forms.ComboBox
            Me.label1 = New System.Windows.Forms.Label
            Me.panelViewers = New System.Windows.Forms.Panel
            Me.wvProcessed = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.wvOriginal = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.statusBar1 = New System.Windows.Forms.StatusBar
            Me.statusFilename = New System.Windows.Forms.StatusBarPanel
            Me.statusImageSize = New System.Windows.Forms.StatusBarPanel
            Me.statusDpi = New System.Windows.Forms.StatusBarPanel
            Me.statusPixelFormat = New System.Windows.Forms.StatusBarPanel
            Me.statusPosition = New System.Windows.Forms.StatusBarPanel
            Me.statusTime = New System.Windows.Forms.StatusBarPanel
            Me.statusInfo = New System.Windows.Forms.StatusBarPanel
            Me.openFileDialog1 = New System.Windows.Forms.OpenFileDialog
            Me.panel1.SuspendLayout()
            Me.panelGrid.SuspendLayout()
            Me.panel2.SuspendLayout()
            Me.panelViewers.SuspendLayout()
            CType(Me.statusFilename, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusImageSize, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusDpi, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusPixelFormat, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusPosition, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusTime, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusInfo, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            ' 
            ' mainMenu1
            ' 
            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.menuProcess, Me.menuHelp})
            ' 
            ' menuFile
            ' 
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFileOpen, Me.menuFileSave, Me.menuItem4, Me.menuExit})
            Me.menuFile.Text = "&File"
            ' 
            ' menuFileOpen
            ' 
            Me.menuFileOpen.Index = 0
            Me.menuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO
            Me.menuFileOpen.Text = "&Open"
            '			Me.menuFileOpen.Click += New System.EventHandler(Me.menuFileOpen_Click);
            ' 
            ' menuFileSave
            ' 
            Me.menuFileSave.Enabled = False
            Me.menuFileSave.Index = 1
            Me.menuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS
            Me.menuFileSave.Text = "&Save Result..."
            '			Me.menuFileSave.Click += New System.EventHandler(Me.menuFileSave_Click);
            ' 
            ' menuItem4
            ' 
            Me.menuItem4.Index = 2
            Me.menuItem4.Text = "-"
            ' 
            ' menuExit
            ' 
            Me.menuExit.Index = 3
            Me.menuExit.Text = "E&xit"
            '			Me.menuExit.Click += New System.EventHandler(Me.menuExit_Click);
            ' 
            ' menuProcess
            ' 
            Me.menuProcess.Enabled = False
            Me.menuProcess.Index = 1
            Me.menuProcess.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuProcessImage, Me.menuProcessSelection})
            Me.menuProcess.Text = "&Process"
            ' 
            ' menuProcessImage
            ' 
            Me.menuProcessImage.Index = 0
            Me.menuProcessImage.Shortcut = System.Windows.Forms.Shortcut.CtrlR
            Me.menuProcessImage.Text = "&Entire Image"
            '			Me.menuProcessImage.Click += New System.EventHandler(Me.menuProcessImage_Click);
            ' 
            ' menuProcessSelection
            ' 
            Me.menuProcessSelection.Index = 1
            Me.menuProcessSelection.Text = "&Selection..."
            '			Me.menuProcessSelection.Click += New System.EventHandler(Me.menuProcessSelection_Click);
            ' 
            ' menuHelp
            ' 
            Me.menuHelp.Index = 2
            Me.menuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuHelpAbout})
            Me.menuHelp.Text = "Help"
            ' 
            ' menuHelpAbout
            ' 
            Me.menuHelpAbout.Index = 0
            Me.menuHelpAbout.Text = "About"
            '			Me.menuHelpAbout.Click += New System.EventHandler(Me.menuHelpAbout_Click);
            ' 
            ' panel1
            ' 
            Me.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.panel1.Controls.Add(Me.panelGrid)
            Me.panel1.Controls.Add(Me.panel2)
            Me.panel1.Dock = System.Windows.Forms.DockStyle.Left
            Me.panel1.Location = New System.Drawing.Point(0, 0)
            Me.panel1.Name = "panel1"
            Me.panel1.Size = New System.Drawing.Size(216, 584)
            Me.panel1.TabIndex = 0
            ' 
            ' panelGrid
            ' 
            Me.panelGrid.Controls.Add(Me.propertyGrid1)
            Me.panelGrid.Controls.Add(Me.btnProcess)
            Me.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill
            Me.panelGrid.Location = New System.Drawing.Point(0, 271)
            Me.panelGrid.Name = "panelGrid"
            Me.panelGrid.Size = New System.Drawing.Size(212, 309)
            Me.panelGrid.TabIndex = 4
            ' 
            ' propertyGrid1
            ' 
            Me.propertyGrid1.CommandsVisibleIfAvailable = True
            Me.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.propertyGrid1.LargeButtons = False
            Me.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar
            Me.propertyGrid1.Location = New System.Drawing.Point(0, 35)
            Me.propertyGrid1.Name = "propertyGrid1"
            Me.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical
            Me.propertyGrid1.Size = New System.Drawing.Size(212, 274)
            Me.propertyGrid1.TabIndex = 1
            Me.propertyGrid1.Text = "propertyGrid1"
            Me.propertyGrid1.ToolbarVisible = False
            Me.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window
            Me.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText
            ' 
            ' btnProcess
            ' 
            Me.btnProcess.Dock = System.Windows.Forms.DockStyle.Top
            Me.btnProcess.Enabled = False
            Me.btnProcess.Location = New System.Drawing.Point(0, 0)
            Me.btnProcess.Name = "btnProcess"
            Me.btnProcess.Size = New System.Drawing.Size(212, 35)
            Me.btnProcess.TabIndex = 0
            Me.btnProcess.Text = "Click Here To Process"
            '			Me.btnProcess.Click += New System.EventHandler(Me.btnProcess_Click);
            ' 
            ' panel2
            ' 
            Me.panel2.Controls.Add(Me.listCommands)
            Me.panel2.Controls.Add(Me.cboFilter)
            Me.panel2.Controls.Add(Me.label1)
            Me.panel2.Dock = System.Windows.Forms.DockStyle.Top
            Me.panel2.Location = New System.Drawing.Point(0, 0)
            Me.panel2.Name = "panel2"
            Me.panel2.Size = New System.Drawing.Size(212, 271)
            Me.panel2.TabIndex = 2
            ' 
            ' listCommands
            ' 
            Me.listCommands.Anchor = (CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
            Me.listCommands.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.listCommands.IntegralHeight = False
            Me.listCommands.Location = New System.Drawing.Point(0, 42)
            Me.listCommands.Name = "listCommands"
            Me.listCommands.Size = New System.Drawing.Size(211, 228)
            Me.listCommands.TabIndex = 2
            '			Me.listCommands.SelectedIndexChanged += New System.EventHandler(Me.listCommands_SelectedIndexChanged);
            ' 
            ' cboFilter
            ' 
            Me.cboFilter.Anchor = (CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
            Me.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboFilter.Items.AddRange(New Object() {"All", "Feature Detection/Removal", "Image Enhancement"})
            Me.cboFilter.Location = New System.Drawing.Point(48, 16)
            Me.cboFilter.Name = "cboFilter"
            Me.cboFilter.Size = New System.Drawing.Size(156, 21)
            Me.cboFilter.Sorted = True
            Me.cboFilter.TabIndex = 0
            '			Me.cboFilter.SelectedIndexChanged += New System.EventHandler(Me.cboFilter_SelectedIndexChanged);
            ' 
            ' label1
            ' 
            Me.label1.Location = New System.Drawing.Point(8, 16)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(48, 21)
            Me.label1.TabIndex = 1
            Me.label1.Text = "Filters:"
            Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            ' 
            ' panelViewers
            ' 
            Me.panelViewers.BackColor = System.Drawing.SystemColors.Control
            Me.panelViewers.Controls.Add(Me.wvProcessed)
            Me.panelViewers.Controls.Add(Me.wvOriginal)
            Me.panelViewers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.panelViewers.Location = New System.Drawing.Point(216, 0)
            Me.panelViewers.Name = "panelViewers"
            Me.panelViewers.Size = New System.Drawing.Size(672, 584)
            Me.panelViewers.TabIndex = 1
            '			Me.panelViewers.Layout += New System.Windows.Forms.LayoutEventHandler(Me.panelViewers_Layout);
            ' 
            ' wvProcessed
            ' 
            Me.wvProcessed.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.Full
            Me.wvProcessed.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.BestFitShrinkOnly
            Me.wvProcessed.BackColor = System.Drawing.Color.FromArgb((CByte(255)), (CByte(192)), (CByte(128)))
            Me.wvProcessed.Centered = True
            Me.wvProcessed.DisplayProfile = Nothing
            Me.wvProcessed.Location = New System.Drawing.Point(377, 8)
            Me.wvProcessed.Magnifier.BackColor = System.Drawing.Color.White
            Me.wvProcessed.Magnifier.BorderColor = System.Drawing.Color.Black
            Me.wvProcessed.Magnifier.Size = New System.Drawing.Size(100, 100)
            Me.wvProcessed.Magnifier.Zoom = 2
            Me.wvProcessed.Name = "wvProcessed"
            Me.wvProcessed.OutputProfile = Nothing
            Me.wvProcessed.Selection = Nothing
            Me.wvProcessed.Size = New System.Drawing.Size(292, 593)
            Me.wvProcessed.TabIndex = 1
            Me.wvProcessed.Text = "workspaceViewer2"
            '			Me.wvProcessed.MouseMovePixel += New System.Windows.Forms.MouseEventHandler(Me.wvProcessed_MouseMovePixel);
            '			Me.wvProcessed.Paint += New System.Windows.Forms.PaintEventHandler(Me.wvProcessed_Paint);
            ' 
            ' wvOriginal
            ' 
            Me.wvOriginal.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.Full
            Me.wvOriginal.AutoZoom = Atalasoft.Imaging.WinControls.AutoZoomMode.BestFitShrinkOnly
            Me.wvOriginal.BackColor = System.Drawing.Color.DodgerBlue
            Me.wvOriginal.Centered = True
            Me.wvOriginal.DisplayProfile = Nothing
            Me.wvOriginal.Location = New System.Drawing.Point(8, 9)
            Me.wvOriginal.Magnifier.BackColor = System.Drawing.Color.White
            Me.wvOriginal.Magnifier.BorderColor = System.Drawing.Color.Black
            Me.wvOriginal.Magnifier.Size = New System.Drawing.Size(100, 100)
            Me.wvOriginal.Magnifier.Zoom = 2
            Me.wvOriginal.Name = "wvOriginal"
            Me.wvOriginal.OutputProfile = Nothing
            Me.wvOriginal.Selection = Nothing
            Me.wvOriginal.Size = New System.Drawing.Size(355, 588)
            Me.wvOriginal.TabIndex = 0
            Me.wvOriginal.Text = "workspaceViewer1"
            '			Me.wvOriginal.MouseMovePixel += New System.Windows.Forms.MouseEventHandler(Me.wvProcessed_MouseMovePixel);
            ' 
            ' statusBar1
            ' 
            Me.statusBar1.Location = New System.Drawing.Point(0, 584)
            Me.statusBar1.Name = "statusBar1"
            Me.statusBar1.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.statusFilename, Me.statusImageSize, Me.statusDpi, Me.statusPixelFormat, Me.statusPosition, Me.statusTime, Me.statusInfo})
            Me.statusBar1.ShowPanels = True
            Me.statusBar1.Size = New System.Drawing.Size(888, 22)
            Me.statusBar1.TabIndex = 2
            Me.statusBar1.Text = "statusBar1"
            ' 
            ' statusFilename
            ' 
            Me.statusFilename.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusFilename.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
            Me.statusFilename.Text = "-"
            Me.statusFilename.ToolTipText = "Filename"
            Me.statusFilename.Width = 272
            ' 
            ' statusImageSize
            ' 
            Me.statusImageSize.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusImageSize.Text = "-"
            Me.statusImageSize.ToolTipText = "Image Size"
            ' 
            ' statusDpi
            ' 
            Me.statusDpi.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusDpi.Text = "-"
            Me.statusDpi.ToolTipText = "Image Resolution"
            ' 
            ' statusPixelFormat
            ' 
            Me.statusPixelFormat.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusPixelFormat.Text = "-"
            Me.statusPixelFormat.ToolTipText = "Pixel Format"
            ' 
            ' statusPosition
            ' 
            Me.statusPosition.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusPosition.Text = "-"
            Me.statusPosition.ToolTipText = "Cursor Position"
            ' 
            ' statusTime
            ' 
            Me.statusTime.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusTime.Text = "-"
            Me.statusTime.ToolTipText = "Processing Time"
            ' 
            ' statusInfo
            ' 
            Me.statusInfo.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            ' 
            ' openFileDialog1
            ' 
            Me.openFileDialog1.Filter = "Images|*.jpg;*.tif;*.png"
            ' 
            ' Form1
            ' 
            Me.AcceptButton = Me.btnProcess
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(888, 606)
            Me.Controls.Add(Me.panelViewers)
            Me.Controls.Add(Me.panel1)
            Me.Controls.Add(Me.statusBar1)
            Me.Menu = Me.mainMenu1
            Me.MinimumSize = New System.Drawing.Size(400, 400)
            Me.Name = "Form1"
            Me.Text = "Atalasoft Advanced Document Cleanup Demo"
            Me.panel1.ResumeLayout(False)
            Me.panelGrid.ResumeLayout(False)
            Me.panel2.ResumeLayout(False)
            Me.panelViewers.ResumeLayout(False)
            CType(Me.statusFilename, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusImageSize, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusDpi, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusPixelFormat, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusPosition, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusTime, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusInfo, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
#End Region

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main()
            Application.Run(New Form1)
        End Sub

        Private Sub cboFilter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFilter.SelectedIndexChanged
            Dim items As String() = Nothing

            Select Case Me.cboFilter.Text
                Case "All"
                    items = New String() {"Automatic Document Negation", "Binary Segmentation", "Black Border Crop", "Blank Page Detection", "Blob Removal", "Border Removal", "Deskew", "Despeckle", "Halftone Removal", "Hole Punch Removal", "Inverted Text Correction", "Line Removal", "Speck Removal", "White Margin Crop"}
                Case "Feature Detection/Removal"
                    items = New String() {"Blank Page Detection", "Blob Removal", "Border Removal", "Halftone Removal", "Hole Punch Removal", "Line Removal", "Speck Removal", "White Margin Crop"}
                Case "Image Enhancement"
                    items = New String() {"Automatic Document Negation", "Deskew", "Despeckle", "Inverted Text Correction"}
                Case "Segmentation"
                    items = New String() {"Binary Segmentation"}
            End Select

            Me.listCommands.Items.Clear()
            Me.listCommands.Items.AddRange(items)
        End Sub

        Private Sub panelViewers_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles panelViewers.Layout
            ' Make both viewers the same size.
            Dim w As Integer = (Me.panelViewers.ClientSize.Width - 24) / 2
            Dim h As Integer = Me.panelViewers.ClientSize.Height - 24

            Me.wvOriginal.SetBounds(8, 8, w, h)
            Me.wvProcessed.SetBounds(w + 16, 8, w, h)

            ' Set the magnifier zoom.
            Me.wvOriginal.Magnifier.Zoom = Me.wvOriginal.Zoom * 4
            Me.wvProcessed.Magnifier.Zoom = Me.wvOriginal.Zoom * 4
        End Sub

        Private Sub listCommands_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles listCommands.SelectedIndexChanged
            Select Case Me.listCommands.Text
                Case "Binary Segmentation"
                    Me.propertyGrid1.SelectedObject = Me._autoBinarize
                Case "Black Border Crop"
                    Me.propertyGrid1.SelectedObject = Me._autoBorderCrop
                Case "Inverted Text Correction"
                    Me.propertyGrid1.SelectedObject = Me._autoInverseTextCorrection
                Case "Automatic Document Negation"
                    Me.propertyGrid1.SelectedObject = Me._autoNegate
                Case "Blank Page Detection"
                    Me.propertyGrid1.SelectedObject = Me._blankPageDetection
                Case "Blob Removal"
                    Me.propertyGrid1.SelectedObject = Me._blobRemoval
                Case "Border Removal"
                    Me.propertyGrid1.SelectedObject = Me._borderRemoval
                Case "Deskew"
                    Me.propertyGrid1.SelectedObject = Me._deskew
                Case "Despeckle"
                    Me.propertyGrid1.SelectedObject = Me._despeckle
                Case "Hole Punch Removal"
                    Me.propertyGrid1.SelectedObject = Me._holePunchRemoval
                Case "White Margin Crop"
                    Me.propertyGrid1.SelectedObject = Me._marginCrop
                Case "Line Removal"
                    Me.propertyGrid1.SelectedObject = Me._lineRemoval
                Case "Speck Removal"
                    Me.propertyGrid1.SelectedObject = Me._speckRemoval
                Case "Halftone Removal"
                    Me.propertyGrid1.SelectedObject = Me._halftoneremoval
                    Exit Select
            End Select

            EnableProcessing((Me.listCommands.SelectedIndex <> -1) AndAlso Not Me.wvOriginal.Image Is Nothing)
        End Sub

        Private Sub EnableProcessing(ByVal enabled As Boolean)
            Me.btnProcess.Enabled = enabled
            Me.menuProcess.Enabled = enabled
        End Sub

        Private Sub menuFileOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileOpen.Click
            Try
                ' try to locate images folder
                Dim imagesFolder As String = Application.ExecutablePath 'System.IO.Directory.GetCurrentDirectory();
                ' we assume we are running under the DotImage install folder
                Dim pos As Integer = imagesFolder.IndexOf("DotImage ")
                If pos <> -1 Then
                    imagesFolder = imagesFolder.Substring(0, imagesFolder.IndexOf("\", pos)) & "\Images\Documents"
                End If

                'use this folder as starting point			
                Me.openFileDialog1.InitialDirectory = imagesFolder

                If Me.openFileDialog1.ShowDialog(Me) = DialogResult.OK Then
                    Me.wvProcessed.Images.Clear()
                    Me.wvProcessed.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.None

                    'is this a multiframe document?
                    Dim frameCount As Integer = RegisteredDecoders.GetImageInfo(Me.openFileDialog1.FileName).FrameCount
                    Dim frameNum As Integer = 0
                    '					if (frameCount > 1)
                    '					{
                    '						FrameNumberDialog fd = new FrameNumberDialog();
                    '						fd.SetNumberOfFrames(frameCount);
                    '						fd.ShowDialog(this);
                    '						frameNum = fd.FrameIndex;
                    '						fd.Dispose();
                    '					}
                    ' show all pages and let user pick
                    If frameCount > 1 Then
                        Dim tf As ThumbnailForm = New ThumbnailForm
                        tf.thumbnailView1.Items.Add(Me.openFileDialog1.FileName, -1, "")
                        If tf.ShowDialog() <> DialogResult.OK Then
                            Return
                        End If
                        frameNum = tf.thumbnailView1.SelectedIndices(0)
                    End If

                    Me.wvOriginal.Open(Me.openFileDialog1.FileName, frameNum)
                    Application.DoEvents()
                    Me.wvOriginal.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.Magnifier

                    Me.statusFilename.Text = Path.GetFileName(Me.openFileDialog1.FileName)
                    Me.statusImageSize.Text = Me.wvOriginal.Image.Width.ToString() & ", " & Me.wvOriginal.Image.Height.ToString()
                    Me.statusInfo.Text = ""
                    Me.statusPixelFormat.Text = Me.wvOriginal.Image.PixelFormat.ToString()
                    Me.statusDpi.Text = Me.wvOriginal.Image.Resolution.X.ToString() & ", " & Me.wvOriginal.Image.Resolution.Y.ToString()
                    Me.statusTime.Text = "-"

                    Me.menuFileSave.Enabled = False
                    EnableProcessing(Me.listCommands.SelectedIndex <> -1)

                    ' Set the magnifier zoom.
                    Me.wvOriginal.Magnifier.Zoom = Me.wvOriginal.Zoom * 4
                    Me.wvProcessed.Magnifier.Zoom = Me.wvOriginal.Zoom * 4
                End If
            Catch ex As Exception
                MessageBox.Show(Me, ex.ToString(), "Exception")
            End Try
        End Sub

        Private Sub menuFileSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileSave.Click
            If Me.wvProcessed.Image Is Nothing Then
                Return
            End If

            Dim dlg As SaveFileDialog = New SaveFileDialog
            dlg.Filter = "TIFF (*.tif)|*.tif"

            Try
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    Dim tif As Atalasoft.Imaging.Codec.TiffEncoder = New Atalasoft.Imaging.Codec.TiffEncoder(Atalasoft.Imaging.Codec.TiffCompression.Group4FaxEncoding, False)
                    If Me.wvProcessed.Image.PixelFormat <> PixelFormat.Pixel1bppIndexed Then
                        tif.Compression = Atalasoft.Imaging.Codec.TiffCompression.Lzw
                    End If

                    Me.wvProcessed.Save(dlg.FileName, tif)
                End If
            Catch ex As Exception
                MessageBox.Show(Me, ex.ToString(), "Exception")
            Finally
                dlg.Dispose()
            End Try
        End Sub

        Private Sub menuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuExit.Click
            Me.Close()
        End Sub

        Private Sub menuProcessImage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuProcessImage.Click
            If Me.wvOriginal.Image Is Nothing OrElse Me.propertyGrid1.SelectedObject Is Nothing Then
                Return
            End If

            Me.wvProcessed.Images.Clear()
            Me.wvProcessed.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.None
            Me.wvProcessed.Refresh()

            If Me._watchSelection Then
                Me._watchSelection = False

                Dim cmd As Atalasoft.Imaging.ImageProcessing.ImageRegionCommand = CType(IIf(TypeOf Me.propertyGrid1.SelectedObject Is Atalasoft.Imaging.ImageProcessing.ImageRegionCommand, Me.propertyGrid1.SelectedObject, Nothing), Atalasoft.Imaging.ImageProcessing.ImageRegionCommand)
                If Not cmd Is Nothing Then
                    cmd.RegionOfInterest = New Atalasoft.Imaging.ImageProcessing.RegionOfInterest(Me.wvOriginal.Selection.Bounds)
                    ProcessCommand(cmd)
                Else
                    ProcessCommand(CType(Me.propertyGrid1.SelectedObject, Atalasoft.Imaging.ImageProcessing.ImageCommand))
                End If

                Me.wvOriginal.Selection.Visible = False
                Me.wvOriginal.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.Magnifier
            Else
                ProcessCommand(CType(Me.propertyGrid1.SelectedObject, Atalasoft.Imaging.ImageProcessing.ImageCommand))
            End If
        End Sub

        Private Sub menuProcessSelection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuProcessSelection.Click
            If Me.wvOriginal.Image Is Nothing OrElse Me.propertyGrid1.SelectedObject Is Nothing Then
                Return
            End If

            Me.wvProcessed.Images.Clear()

            If (Not Me._selectionMessageShown) Then
                MessageBox.Show(Me, "Use the mouse to select an area of the image the click the 'Process' button.", "Select Area", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me._selectionMessageShown = True
            End If

            Me.wvOriginal.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.Selection
            Me._watchSelection = True
        End Sub

        Private Sub ProcessCommand(ByVal cmd As Atalasoft.Imaging.ImageProcessing.ImageCommand)
            Me.Cursor = Cursors.WaitCursor
            Dim t1 As Integer = 0, t2 As Integer = 0
            Dim icResults As ImageResults = Nothing
            Me.statusInfo.Text = Nothing

            Try
                If cmd.InPlaceProcessing Then
                    Dim img As AtalaImage = CType(Me.wvOriginal.Image.Clone(), AtalaImage)
                    t1 = System.Environment.TickCount
                    icResults = cmd.Apply(img)
                    t2 = System.Environment.TickCount - t1
                    Me.wvProcessed.Images.Add(img)
                Else
                    t1 = System.Environment.TickCount
                    icResults = cmd.Apply(Me.wvOriginal.Image)
                    Dim img As AtalaImage = icResults.Image
                    If Not img Is Nothing Then
                        wvProcessed.Images.Add(img)
                    End If
                    t2 = System.Environment.TickCount - t1
                End If

                Select Case cmd.GetType().Name
                    Case "BlankPageDetectionCommand"
                        Dim bpdResults As BlankPageDetectionResults = CType(icResults, BlankPageDetectionResults)
                        If bpdResults.IsImageBlank Then
                            Me.statusInfo.Text = ("BLANK")
                        Else
                            Me.statusInfo.Text = ("NOT BLANK")
                        End If
                    Case "AutoNegateCommand"
                        Dim anResults As AutoNegateResults = CType(icResults, AutoNegateResults)
                        If anResults.IsInverted Then
                            Me.statusInfo.Text = ("INVERTED")
                        Else
                            Me.statusInfo.Text = ("NOT INVERTED")
                        End If
                    Case "AutoDeskewCommand"
                        Dim adResults As AutoDeskewResults = CType(icResults, AutoDeskewResults)
                        Me.statusInfo.Text = adResults.SkewAngle.ToString() & " deg"
                End Select

            Catch e As Exception
                MessageBox.Show(e.ToString())
            Finally
                Me.Cursor = Cursors.Default
                Me.wvProcessed.MouseTool = Atalasoft.Imaging.WinControls.MouseToolType.Magnifier
                Me.statusTime.Text = t2.ToString() & " msec"
                Me.menuFileSave.Enabled = True
            End Try
        End Sub

        Private Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
            Me.menuProcessImage_Click(sender, e)
        End Sub

        Private Sub wvProcessed_MouseMovePixel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles wvProcessed.MouseMovePixel, wvOriginal.MouseMovePixel
            Me.statusPosition.Text = e.X.ToString() & ", " & e.Y.ToString()
        End Sub

        Private Sub menuHelpAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuHelpAbout.Click
            Dim aboutBox As AtalaDemos.AboutBox.About = New AtalaDemos.AboutBox.About("About Atalasoft DotImage Advanced Document Cleanup Demo", "Advanced Document Cleanup Demo")
            aboutBox.Description = "Demonstrates each of the Document Cleanup and processing commands available in the DotImage Advanced Document Cleanup Module.  Shows the before and after images side by side and provides access to each property of each available command.  Requires a license of DotImage Document Imaging and DotImage Advanced Document Cleanup to run the compiled demo."
            aboutBox.ShowDialog()
        End Sub

        Private Function GetRegionSpecificPen(ByVal index As Integer) As Pen
            Return New Pen(_pal.GetEntry(index), 2.0F)
        End Function
        Private Function GetScrolledPoint(ByVal p As PointF, ByVal sp As Point, ByVal zoomX As Double, ByVal zoomY As Double) As Point
            Return New Point(sp.X + CInt(p.X * zoomX), sp.Y + CInt(p.Y * zoomY))
        End Function
        Private Function GetScrolledRectangle(ByVal r As Rectangle, ByVal sp As Point, ByVal zoomX As Double, ByVal zoomY As Double) As Rectangle
            Return New Rectangle(sp.X + CInt(r.Left * zoomX), sp.Y + CInt(r.Top * zoomY), CInt(r.Width * zoomX), CInt(r.Height * zoomY))
        End Function

        Private _pal As Palette = Nothing
        '
        Private Sub wvProcessed_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles wvProcessed.Paint
            If Me.wvProcessed.Image Is Nothing Then
                Return
            End If
            Dim controlSize As Size = Me.wvProcessed.ScrollSize
            Dim zoomx As Double = CDbl(controlSize.Width) / CDbl(Me.wvProcessed.Image.Size.Width)
            Dim zoomy As Double = CDbl(controlSize.Height) / CDbl(Me.wvProcessed.Image.Size.Height)
            Dim sp As Point = wvProcessed.ScrollPosition
            If _pal Is Nothing Then
                _pal = New Palette(PaletteType.WebExtended)
            End If

        End Sub
    End Class
End Namespace
