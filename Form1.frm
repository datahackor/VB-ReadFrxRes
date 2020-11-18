VERSION 5.00
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "COMDLG32.OCX"
Begin VB.Form Form1 
   Caption         =   "读取Frx Ctx文件"
   ClientHeight    =   7995
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   10140
   LinkTopic       =   "Form1"
   ScaleHeight     =   7995
   ScaleWidth      =   10140
   StartUpPosition =   3  '窗口缺省
   Begin MSComDlg.CommonDialog CDialog1 
      Left            =   8160
      Top             =   6840
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin MSComDlg.CommonDialog CDOpen 
      Left            =   6960
      Top             =   6840
      _ExtentX        =   847
      _ExtentY        =   847
      _Version        =   393216
   End
   Begin VB.CommandButton cmdOpenFile 
      Caption         =   "打开文件"
      Height          =   495
      Left            =   2760
      TabIndex        =   3
      Top             =   6600
      Width           =   1215
   End
   Begin VB.CommandButton cmdSavePicture 
      Caption         =   "保存图片"
      Height          =   495
      Left            =   4800
      TabIndex        =   2
      Top             =   6600
      Width           =   1215
   End
   Begin VB.ListBox List1 
      Height          =   5640
      Left            =   120
      TabIndex        =   1
      Top             =   120
      Width           =   3975
   End
   Begin VB.PictureBox Picture1 
      BorderStyle     =   0  'None
      Height          =   5655
      Left            =   4200
      ScaleHeight     =   5655
      ScaleWidth      =   5415
      TabIndex        =   0
      Top             =   120
      Width           =   5415
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Dim strFilePath As String       'frx,ctx
Dim strTextFilePath As String   'frm,ctl
Dim strFileName As String       '二进制资源文件名

Dim m_strKey() As String
Dim m_lngOffset() As Long       '记录一个文件的全部偏移数据

Private Sub cmdOpenFile_Click()
    Dim strFileExt As String
    
    CDialog1.Filter = "VB二进制资源文件(*.frx;*.ctx)|*.frx;*.ctx|所有文件(*.*)|*.*"
    CDialog1.ShowOpen
    If CDialog1.FileName = "" Then Exit Sub
    strFilePath = CDialog1.FileName
    strFileName = ExtractFileName(strFilePath)
    
    strFileExt = LCase$(ExtractFileExt(strFilePath))
    If strFileExt = "frx" Then
        strTextFilePath = ExtractMainFileName(strFileName) & ".frm"
    ElseIf strFileExt = "ctx" Then
        strTextFilePath = ExtractMainFileName(strFileName) & ".ctl"
    Else
        MsgBox "不支持该种格式文件！"
    End If
    
    ReadFrm strTextFilePath
    
    
End Sub


Sub ReadFrm(ByVal FileName As String)
    Dim strLine As String
    Dim strArray() As String
    Dim strArrayOffset() As String
    
    Dim strKey As String
    Dim lngOffset As Long
    Dim i As Integer
    
    List1.Clear
    
    Open FileName For Input As #1
    Do Until EOF(1)
        Line Input #1, strLine
    
        If InStr(1, strLine, strFileName) Then
            strArray = Split(strLine, "=")
            strKey = Trim$(strArray(0))    '关键字
            strArrayOffset() = Split(strArray(1), ":")
            lngOffset = CLng("&H" & Trim$(strArrayOffset(1)))
            
            ReDim Preserve m_strKey(i)
            m_strKey(i) = strKey
            ReDim Preserve m_lngOffset(i)
            m_lngOffset(i) = lngOffset
            Debug.Print m_strKey(i), m_lngOffset(i)
            i = i + 1
            
            List1.AddItem strKey & IIf(Len(strKey) >= 8, vbTab, vbTab & vbTab) & Hex$(lngOffset)
        End If
    Loop
    Close #1
    
End Sub

Private Sub cmdSavePicture_Click()
    Dim lngSize As Long
    Dim bytData() As Byte
    Dim lngTmp As Long      '读取比较标志
    
    Const FlagPicture = &H746C  '29804      '8
    Const FlagImageList = &HBE35204         '16

    CDialog1.Filter = "全部文件(*.*)|*.*"
    CDialog1.FileName = ""
    CDialog1.ShowSave
    If CDialog1.FileName = "" Then Exit Sub
    
    
    Open strFilePath For Binary As #1
    Seek #1, m_lngOffset(List1.ListIndex) + 1       '注意，这里要加1，第1个字节是从1开始的
    Get #1, , lngSize
    'Picture控件的Picture属性，后面带着8个字节的数据
    'picture    6C 74 00 00 7E 01 00 00
    'imagelist  04 52 E3 0B 91 8F CE 11 9D E3 00 AA 00 4B  B8 51
    'image      6C 74 00 00 48 42 00 00     '可能后面4位字节的数据是不同的
    
    'Picture可能后面随着控件的不同会附着一些数据，再后面才是图片数据
    Get #1, , lngTmp
    If lngTmp = FlagPicture Then
        Get #1, , lngTmp    '再跳过4位
        ReDim bytData(lngSize - 1 - 8)
        Get #1, , bytData
    
    ElseIf lngTmp = FlagImageList Then
        Seek #1, Seek(1) + 16 + 4
        ReDim bytData(lngSize - 1 - 16 - 8)
        Get #1, , bytData
    Else
    End If
    Close #1
    
    Open CDialog1.FileName For Binary As #1
    Put #1, , bytData
    Close #1
    
    MsgBox "Over"
    
End Sub

Private Sub Form_Resize()
    Picture1.Left = List1.Left + List1.Width
    Picture1.Width = Me.Width - List1.Left - List1.Width - 100
    
End Sub

Private Sub List1_Click()
On Error GoTo Err1

    Dim lngSize As Long
    Dim bytData() As Byte
    Dim lngTmp As Long      '读取比较标志
    
    Const FlagPicture = &H746C  '29804      '8
    Const FlagImageList = &HBE35204         '16
    
    Picture1.Picture = LoadPicture()
    
    Select Case LCase$(m_strKey(List1.ListIndex))
    Case "picture", "icon", "mouseicon", "toolboxbitmap"
    
    'Picture
    
        Open strFilePath For Binary As #1
        Seek #1, m_lngOffset(List1.ListIndex) + 1       '注意，这里要加1，第1个字节是从1开始的
        Get #1, , lngSize
        'Picture控件的Picture属性，后面带着8个字节的数据
        'picture    6C 74 00 00 7E 01 00 00
        'imagelist  04 52 E3 0B 91 8F CE 11 9D E3 00 AA 00 4B  B8 51
        'image      6C 74 00 00 48 42 00 00     '可能后面4位字节的数据是不同的
        
        'Picture可能后面随着控件的不同会附着一些数据，再后面才是图片数据
        Get #1, , lngTmp
        If lngTmp = FlagPicture Then
            Get #1, , lngTmp    '再跳过4位
            ReDim bytData(lngSize - 1 - 8)
            Get #1, , bytData
            Picture1.Picture = BytesToPicture(bytData)
        
        ElseIf lngTmp = FlagImageList Then
            Seek #1, Seek(1) + 16 + 4
            ReDim bytData(lngSize - 1 - 16 - 8)
            Get #1, , bytData
            Picture1.Picture = BytesToPicture(bytData)
        Else
        End If
        Close #1
        '说明,使用BytesToPicture不能显示ICO文件和Cur文件,其它一般的图片格式可以正常显示
        
        
        
    Case Else
    End Select
    
    Exit Sub
    
Err1:
    Close
    MsgBox Err.Description, vbCritical, Err.Number
    
End Sub
