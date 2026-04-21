Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace AdvancedDocClean
	''' <summary>
	''' Summary description for FrameNumberDialog.
	''' </summary>
	Public Class FrameNumberDialog : Inherits System.Windows.Forms.Form
		Private WithEvents comboBox1 As System.Windows.Forms.ComboBox
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing

		Public Sub New()
			'
			' Required for Windows Form Designer support
			'
			InitializeComponent()

			'
			' TODO: Add any constructor code after InitializeComponent call
			'
		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
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
			Me.comboBox1 = New System.Windows.Forms.ComboBox()
			Me.SuspendLayout()
			' 
			' comboBox1
			' 
			Me.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.comboBox1.Location = New System.Drawing.Point(24, 16)
			Me.comboBox1.Name = "comboBox1"
			Me.comboBox1.Size = New System.Drawing.Size(96, 21)
			Me.comboBox1.TabIndex = 1
'			Me.comboBox1.SelectedIndexChanged += New System.EventHandler(Me.comboBox1_SelectedIndexChanged);
			' 
			' FrameNumberDialog
			' 
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(154, 56)
			Me.ControlBox = False
			Me.Controls.Add(Me.comboBox1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.Name = "FrameNumberDialog"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Select Frame Number"
			Me.ResumeLayout(False)

		End Sub
		#End Region

		Private Sub comboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles comboBox1.SelectedIndexChanged
			Me.Hide()
		End Sub

		Public ReadOnly Property FrameIndex() As Integer
			Get
				Return comboBox1.SelectedIndex
			End Get
		End Property

		Public Sub SetNumberOfFrames(ByVal frames As Integer)
			comboBox1.Items.Clear()
			Dim i As Integer = 0
			Do While i < frames
				comboBox1.Items.Add(i)
				i += 1
			Loop
		End Sub
	End Class
End Namespace
