Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace AdvancedDocClean
	''' <summary>
	''' Summary description for ThumbnailForm.
	''' </summary>
	Public Class ThumbnailForm : Inherits System.Windows.Forms.Form
		Public thumbnailView1 As Atalasoft.Imaging.WinControls.ThumbnailView
		Private button1 As System.Windows.Forms.Button
		Private label1 As System.Windows.Forms.Label
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
            Me.thumbnailView1 = New Atalasoft.Imaging.WinControls.ThumbnailView
            Me.button1 = New System.Windows.Forms.Button
            Me.label1 = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'thumbnailView1
            '
            Me.thumbnailView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.thumbnailView1.BackColor = System.Drawing.Color.Silver
            Me.thumbnailView1.DragSelectionColor = System.Drawing.Color.Red
            Me.thumbnailView1.ForeColor = System.Drawing.SystemColors.WindowText
            Me.thumbnailView1.HighlightBackgroundColor = System.Drawing.SystemColors.Highlight
            Me.thumbnailView1.HighlightTextColor = System.Drawing.SystemColors.HighlightText
            Me.thumbnailView1.LoadErrorMessage = ""
            Me.thumbnailView1.Location = New System.Drawing.Point(8, 32)
            Me.thumbnailView1.Margins = New Atalasoft.Imaging.WinControls.Margin(4, 4, 4, 4)
            Me.thumbnailView1.Name = "thumbnailView1"
            Me.thumbnailView1.SelectedItemStyle = Atalasoft.Imaging.WinControls.SelectedItemRenderStyle.Extended
            Me.thumbnailView1.SelectionRectangleBackColor = System.Drawing.Color.Transparent
            Me.thumbnailView1.SelectionRectangleDashStyle = System.Drawing.Drawing2D.DashStyle.Solid
            Me.thumbnailView1.SelectionRectangleLineColor = System.Drawing.Color.Black
            Me.thumbnailView1.Size = New System.Drawing.Size(424, 400)
            Me.thumbnailView1.TabIndex = 0
            Me.thumbnailView1.Text = "thumbnailView1"
            Me.thumbnailView1.ThumbnailBackground = Nothing
            Me.thumbnailView1.ThumbnailOffset = New System.Drawing.Point(0, 0)
            Me.thumbnailView1.ThumbnailSize = New System.Drawing.Size(120, 120)
            '
            'button1
            '
            Me.button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.button1.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.button1.Location = New System.Drawing.Point(244, 448)
            Me.button1.Name = "button1"
            Me.button1.Size = New System.Drawing.Size(71, 23)
            Me.button1.TabIndex = 1
            Me.button1.Text = "OK"
            '
            'label1
            '
            Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label1.Location = New System.Drawing.Point(0, 8)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(256, 23)
            Me.label1.TabIndex = 2
            Me.label1.Text = "Please Select a frame Index to Open:"
            '
            'ThumbnailForm
            '
            Me.AcceptButton = Me.button1
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(440, 478)
            Me.Controls.Add(Me.button1)
            Me.Controls.Add(Me.thumbnailView1)
            Me.Controls.Add(Me.label1)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ThumbnailForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Open Multipage Image"
            Me.ResumeLayout(False)

        End Sub
		#End Region
	End Class
End Namespace
