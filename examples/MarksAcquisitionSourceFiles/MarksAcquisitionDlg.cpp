// MarksAcquisitionDlg.cpp : implementation file
//

#include "stdafx.h"
#include "MarksAcquisition.h"
#include "MarksAcquisitionDlg.h"
#include <string>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About


//MDSTODO
// Why is 30sec a count of 33 (on laptop)
// Have # Exposures count down during start&stop
// Hit OK to low battery window (is this an easy to find child window or does it need window name and control ID)
// Make Window name and control ID configurable
// Save state in an ini file
// Self checking timings?

//v1.2 controls Imaging Edge and also cancels the Battery Low Dialog



class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMarksAcquisitionDlg dialog

CMarksAcquisitionDlg::CMarksAcquisitionDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMarksAcquisitionDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMarksAcquisitionDlg)
	mExpLength = 30;
	mInterframe = 1;
	mNumFrames = 1000;
	m_PhdDitherAmount = 5;
	m_PhdSettleAccuracy = 0.3;
	m_PhdDitherTimeout = 15;
	m_DitherExposures = 1;
	m_PhdPreSettle = 0;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	mStatus = 0;
	mPhdConnected = false;

	
	//Get Values from ini file

    char ini_filename[MAX_PATH];
    GetModuleFileName( NULL, ini_filename, MAX_PATH );
	std::string::size_type pos = std::string( ini_filename ).find_last_of( "." );
		
	sprintf(&ini_filename[pos+1], "ini");

	mExpLength  = GetPrivateProfileInt("MAIN","EXPLENGTH",30,ini_filename);
	mInterframe = GetPrivateProfileInt("MAIN","EXPDELAY",2,ini_filename);
	mNumFrames = GetPrivateProfileInt("MAIN","NUMEXP",1000,ini_filename);
	m_PhdDitherAmount = GetPrivateProfileInt("MAIN","DITHERAMOUNT",5,ini_filename);
	m_PhdPreSettle = GetPrivateProfileInt("MAIN","PRESETTLE",2,ini_filename);
	int temp = GetPrivateProfileInt("MAIN","SETTLEAACCURACY",30,ini_filename);
	m_PhdSettleAccuracy = temp/100.0;
	m_PhdDitherTimeout = GetPrivateProfileInt("MAIN","SETTLETIMEOUT",15,ini_filename);
	m_DitherExposures = GetPrivateProfileInt("MAIN","NUMDITHEREXP",2,ini_filename);

	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}
void CMarksAcquisitionDlg::WriteIni()
{

	//Set Values in ini file
    char ini_filename[MAX_PATH];
    GetModuleFileName( NULL, ini_filename, MAX_PATH );
	std::string::size_type pos = std::string( ini_filename ).find_last_of( "." );
	sprintf(&ini_filename[pos+1], "ini");

	char buf[100];
	sprintf(buf,"%d",mExpLength);
	BOOL ret = WritePrivateProfileString("MAIN","EXPLENGTH",buf,ini_filename);
	sprintf(buf,"%d",mInterframe);
	ret = WritePrivateProfileString("MAIN","EXPDELAY",buf,ini_filename);
	sprintf(buf,"%d",mNumFrames);
	ret = WritePrivateProfileString("MAIN","NUMEXP",buf,ini_filename);
	sprintf(buf,"%d",m_PhdDitherAmount);
	ret = WritePrivateProfileString("MAIN","DITHERAMOUNT",buf,ini_filename);
	sprintf(buf,"%d",m_PhdPreSettle);
	ret = WritePrivateProfileString("MAIN","PRESETTLE",buf,ini_filename);
	sprintf(buf,"%d",int(m_PhdSettleAccuracy*100.0 +0.5));
	ret = WritePrivateProfileString("MAIN","SETTLEAACCURACY",buf,ini_filename);
	sprintf(buf,"%d",m_PhdDitherTimeout);
	ret = WritePrivateProfileString("MAIN","SETTLETIMEOUT",buf,ini_filename);
	sprintf(buf,"%d",m_DitherExposures);
	ret = WritePrivateProfileString("MAIN","NUMDITHEREXP",buf,ini_filename);

}

void CMarksAcquisitionDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMarksAcquisitionDlg)
	DDX_Text(pDX, IDC_EDIT_EXPLENGTH, mExpLength);
	DDV_MinMaxInt(pDX, mExpLength, 1, 3600);
	DDX_Text(pDX, IDC_EDIT_INTERFRAME, mInterframe);
	DDV_MinMaxInt(pDX, mInterframe, 0, 60);
	DDX_Text(pDX, IDC_EDIT_NUMEXP, mNumFrames);
	DDV_MinMaxInt(pDX, mNumFrames, 1, 10000);
	DDX_Text(pDX, IDC_EDIT_PHDDITHERAMOUNT, m_PhdDitherAmount);
	DDV_MinMaxInt(pDX, m_PhdDitherAmount, 1, 5);
	DDX_Text(pDX, IDC_EDIT_PHDSETTLEACCURACY, m_PhdSettleAccuracy);
	DDV_MinMaxDouble(pDX, m_PhdSettleAccuracy, 5.e-002, 1.);
	DDX_Text(pDX, IDC_EDIT_PHDDITHERTIMEOUT, m_PhdDitherTimeout);
	DDV_MinMaxInt(pDX, m_PhdDitherTimeout, 1, 120);
	DDX_Text(pDX, IDC_EDIT_DITHEREXPOSURES, m_DitherExposures);
	DDV_MinMaxInt(pDX, m_DitherExposures, 1, 1000);
	DDX_Text(pDX, IDC_EDIT_PRESETTLE, m_PhdPreSettle);
	DDV_MinMaxInt(pDX, m_PhdPreSettle, 1, 100);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CMarksAcquisitionDlg, CDialog)
	//{{AFX_MSG_MAP(CMarksAcquisitionDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_EN_CHANGE(IDC_EDIT_NUMEXP, OnChangeEditNumexp)
	ON_BN_CLICKED(IDC_BUTTON_START, OnButtonStart)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDC_BUTTON_ABORT, OnButtonAbort)
	ON_BN_CLICKED(IDC_BUTTON_PHDCONNECT, OnButtonPhdconnect)
	ON_EN_CHANGE(IDC_EDIT_EXPLENGTH, OnChangeEditExplength)
	ON_EN_CHANGE(IDC_EDIT_INTERFRAME, OnChangeEditInterframe)
	ON_EN_CHANGE(IDC_EDIT_PHDDITHERTIMEOUT, OnChangeEditPhddithertimeout)
	ON_EN_CHANGE(IDC_EDIT_PHDDITHERAMOUNT, OnChangeEditPhdditheramount)
	ON_EN_KILLFOCUS(IDC_EDIT_PHDSETTLEACCURACY, OnChangeEditPhdsettleaccuracy)
	ON_EN_CHANGE(IDC_EDIT_DITHEREXPOSURES, OnChangeEditDitherExposures)
	ON_EN_CHANGE(IDC_EDIT_PRESETTLE, OnChangeEditPresettle)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMarksAcquisitionDlg message handlers

BOOL CMarksAcquisitionDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);


	return TRUE;  // return TRUE  unless you set the focus to a control
}

BOOL CMarksAcquisitionDlg::PostShutterButtonMessage(UINT message)
{
		LPCTSTR lpClassName = NULL;
		LPCTSTR lpWindowName = "Remote Camera Control";
		CWnd* rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
		BOOL ret = true;
		if (rccwin==NULL)
		{
			LPCTSTR lpWindowName = "Remote";  //Imaging Edge
			CWnd* rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
			if (rccwin==NULL)
			{
				KillTimer(99);
				this->MessageBox("Unable to find Sony Remote Camera Control or Imaging Edge", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
				ret = false;
			}
			else
			{
				const int ButtonId = 0x3E9;
				//CWnd* hWndButton = rccwin->GetDlgItem(ButtonId);
				CWnd* hWndButton = rccwin->GetDescendantWindow(ButtonId);
				if (hWndButton==NULL)
				{
					KillTimer(99);
					this->MessageBox("Unable to find button on Sony Imaging Edge", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
					ret = false;
				}
				else
				{
					int wParam = NULL;
					int lParam = NULL;
					BOOL success = hWndButton->PostMessage(message, wParam, lParam);
					if (success == false)
					{
						KillTimer(99);
						this->MessageBox("Unable to PostMessage to button on Sony Imaging Edge", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
						ret = false;
					}
				}
			}
		}
		else
		{
			const int ButtonId = 0x3EE;
			//CWnd* hWndButton = rccwin->GetDlgItem(ButtonId);
			CWnd* hWndButton = rccwin->GetDescendantWindow(ButtonId);
			if (hWndButton==NULL)
			{
				//If there's a dialog button then press it
				const int OkButtonId = 0x2;
				CWnd* hWndOkButton = rccwin->GetDescendantWindow(OkButtonId);
				if (hWndOkButton!=NULL)
				{
					int wParam = NULL;
					int lParam = NULL;
					BOOL success = hWndOkButton->SendMessage(WM_LBUTTONDOWN, wParam, lParam);
					success = hWndOkButton->SendMessage(WM_LBUTTONUP, wParam, lParam);
					Sleep(500);
				}
			}
			rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
			hWndButton = rccwin->GetDescendantWindow(ButtonId);
			if (hWndButton==NULL)
			{
				//If there's a dialog button then press it
				const int OkButtonId = 0x2;
				CWnd* hWndOkButton = rccwin->GetDescendantWindow(OkButtonId);
				if (hWndOkButton!=NULL)
				{
					int wParam = NULL;
					int lParam = NULL;
					BOOL success = hWndOkButton->SendMessage(WM_LBUTTONDOWN, wParam, lParam);
					success = hWndOkButton->SendMessage(WM_LBUTTONUP, wParam, lParam);
					Sleep(500);
				}
			}
		
			rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
			hWndButton = rccwin->GetDescendantWindow(ButtonId);
			if (hWndButton==NULL)
			{
				KillTimer(99);
				this->MessageBox("Unable to find button on Sony Remote Camera Control", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
				ret = false;
			}
			else
			{
				int wParam = NULL;
				int lParam = NULL;
				BOOL success = hWndButton->PostMessage(message, wParam, lParam);
				if (success == false)
				{
					KillTimer(99);
					this->MessageBox("Unable to PostMessage to button on Sony Remote Camera Control", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
					ret = false;
				}

			}
		}
		return ret;
}
void CMarksAcquisitionDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CMarksAcquisitionDlg::OnPaint() 
{



	//(this->GetDlgItem(IDC_STATIC_EXPINFO))  ->EnableWindow(false);
	//(this->GetDlgItem(IDC_STATIC_PHDINFO))  ->EnableWindow(false);




	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}

	
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMarksAcquisitionDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CMarksAcquisitionDlg::OnTimer(UINT nIDEvent) 
{
	if (mStatus == 0)
	{
		BOOL ret = PostShutterButtonMessage(WM_LBUTTONDOWN);
		//Tidy up after error
		if (ret == false)
		{
			KillTimer(99);
			CWnd* pWnd;
			pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
			pWnd->SetWindowText("Sony RCC error");
			(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
			(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
			return;
		}
		mStatus = 1; //Exposure in progress
		mFrameCounter = 0;
		mSecondsCounter = 0;
		mDitherExposureCount = 0;
	}
	else if (mStatus==1)  	{

		//CWnd* foregroundWnd = CWnd::GetForegroundWindow();
		
		//	if (mSecondsCounter==0)
		//	{
		//		BOOL ret = PostShutterButtonMessage(WM_LBUTTONDOWN)
        //
		//   }

		Sleep(150);
			
		mSecondsCounter++;
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
		char buf[100];
		sprintf(buf, "Exposure %i of %i\nAcquisition %i secs\0",mFrameCounter+1,mNumFrames,mSecondsCounter);
		pWnd->SetWindowText(buf);

		if (mSecondsCounter >= mExpLength)
		{

			BOOL ret = PostShutterButtonMessage(WM_LBUTTONUP);
			//Tidy up after error
			if (ret == false)
			{
				KillTimer(99);
				pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
				pWnd->SetWindowText("Sony RCC error");
				(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
				(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
				return;
			}

			mStatus = 2; //Interframe wait
			mSecondsCounter = 0;
			mDitherStarted = false;
		}
		
		

	}
	else if (mStatus==2) //Interframe wait
	{
		// If PHD connected, send dither before doing interframe countdown
		if (mSecondsCounter==0 && mPhdConnected && ((mFrameCounter+1) % m_DitherExposures)==0)
		{

			/*
			From Craig Stark's code:
			1	MSG_PAUSE = 1,
			2	MSG_RESUME, 
			3	MSG_MOVE1,
			4	MSG_MOVE2,
			5	MSG_MOVE3,
			6	MSG_IMAGE,
			7	MSG_GUIDE,
			8	MSG_CAMCONNECT,
			9	MSG_CAMDISCONNECT,
			10	MSG_REQDIST,
			11	MSG_REQFRAME,
			12	MSG_MOVE4,
			13	MSG_MOVE5
			*/

			char buf[100];
			if (m_PhdDitherAmount == 1) buf[0] = 3;  //MSG_MOVE1
			if (m_PhdDitherAmount == 2) buf[0] = 4;  //MSG_MOVE2
			if (m_PhdDitherAmount == 3) buf[0] = 5;  //MSG_MOVE3
			if (m_PhdDitherAmount == 4) buf[0] = 12; //MSG_MOVE4
			if (m_PhdDitherAmount == 5) buf[0] = 13; //MSG_MOVE5

			//TODO GetLastError
			CWnd* pWnd;
			pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
			pWnd->SetWindowText("Sending dither to PHD");
			int flag = mSockClient.Send(buf,1);
			if (flag != 1)
			{
					//TODO GetLastError
					CWnd* pWnd;
					pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
					pWnd->SetWindowText("Unsuccessful send to PHD");
					return;
			}


			Sleep(500);
			flag = mSockClient.Receive(buf,1);
			if (flag != 1)
			{
					//TODO GetLastError
					CWnd* pWnd;
					pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
					pWnd->SetWindowText("Unsuccessful receive from PHD");
					return;
			}
			mDitherStarted = true;
		}
		
		mSecondsCounter++;
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
		char buf[100];
		sprintf(buf, "Exposure %i of %i completed\nDelay %i secs\0",mFrameCounter+1,mNumFrames,mSecondsCounter);
		if (mSecondsCounter <= mInterframe) pWnd->SetWindowText(buf);
		if (mSecondsCounter >= mInterframe && mSecondsCounter >= 2)   
		{
			//Interframe countdown is now complete.  Either acquistion is complete or we enter new mode
			mFrameCounter++;
			if (mFrameCounter >= mNumFrames)
			{
				//ACQUISITION COMPLETE
				KillTimer(99);
				pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
				pWnd->SetWindowText("ACQUISITION COMPLETE");
				(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
				(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
				if (!mPhdConnected)
					(this->GetDlgItem(IDC_BUTTON_PHDCONNECT))  ->EnableWindow(true);
			}
			else
			{
				if (mDitherStarted)
				{
					mStatus = 3; //Waiting for Dither
					mSecondsCounter = 0;
				}
				else
				{
					if (mPhdConnected)
					{
						CWnd* pWnd;
						pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
						pWnd->SetWindowText("Guiding");
					}

					BOOL ret = PostShutterButtonMessage(WM_LBUTTONDOWN);   //TODO check if down is correct
					//Tidy up after error
					if (ret == false)
					{
						KillTimer(99);
						pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
						pWnd->SetWindowText("Sony RCC error");
						(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
						(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
						return;
					}

					mStatus = 1; //Exposure in progress
					mSecondsCounter = 0;
				}
			}

		}
	}
	else if (mStatus==3) //Waiting for Pre-settle (this extra step forces the dither to take placebefore taking a reading) 
	{
		mSecondsCounter++;
		char buf[100];
		sprintf(buf, "Exposure %i of %i pending\nPre-settle delay %i sec",mFrameCounter+1,mNumFrames,mSecondsCounter);
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
		pWnd->SetWindowText(buf);
		if (mSecondsCounter == m_PhdPreSettle)
		{
			mStatus = 4; //Waiting for guiding to settle
			mSecondsCounter = 0;
		}


	}	
	else if (mStatus==4) //Waiting for guiding to settle
	{
		mSecondsCounter++;
		unsigned char buf[1];
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
		char buff[100];
		sprintf(buff, "Exposure %i of %i pending\nAwaiting PHD to settle %i sec",mFrameCounter+1,mNumFrames,mSecondsCounter);
		pWnd->SetWindowText(buff);

		buf[0] = 10;
		int flag = mSockClient.Send(buf,1);
		if (flag != 1)
		{
				//TODO GetLastError
				CWnd* pWnd;
				pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
				pWnd->SetWindowText("Unsuccessful send to PHD");
				return;
		}

		Sleep(500);
		flag = mSockClient.Receive(buf,1);
		if (flag != 1)
		{
				//TODO GetLastError
				CWnd* pWnd;
				pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
				pWnd->SetWindowText("Unsuccessful receive from PHD");
				return;
		}

		double distance = double(buf[0]/100.0);

		pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
		if (distance < 2.5)
			sprintf(buff, "Guide star distance: %5.2f", distance);
		else
			sprintf(buff, "Guide star distance greater than 2.5");
		pWnd->SetWindowText(buff);


		if (distance < m_PhdSettleAccuracy)
		{
			sprintf(buff, "Guiding");
			pWnd->SetWindowText(buff);
					mStatus = 1; //Exposure in progress
					mSecondsCounter = 0;

					BOOL ret = PostShutterButtonMessage(WM_LBUTTONDOWN);
					//Tidy up after error
					if (ret == false)
					{
						KillTimer(99);
						pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
						pWnd->SetWindowText("Sony RCC error");
						(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
						(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
						return;
					}
			

		}
		if (mSecondsCounter > m_PhdDitherTimeout)
		{
			sprintf(buff, "Guiding - settle accuracy was not achieved before timeout");
			pWnd->SetWindowText(buff);


			//Start new exposure
			BOOL ret = PostShutterButtonMessage(WM_LBUTTONDOWN);
			//Tidy up after error
			if (ret == false)
			{
				KillTimer(99);
				pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
				pWnd->SetWindowText("Sony RCC error");
				(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
				(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
				return;
			}
			mStatus = 1; //Exposure in progress
			mSecondsCounter = 0;
		}
	}
}




void CMarksAcquisitionDlg::OnButtonStart() 
{

	//Check shutter button exists but without activating it
	LPCTSTR lpClassName = NULL;
	LPCTSTR lpWindowName = "Remote Camera Control";
	CWnd* rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
	if (rccwin==NULL)
	{
		LPCTSTR lpWindowName = "Remote";  //Imaging Edge
		CWnd* rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
		if (rccwin==NULL)
		{
			this->MessageBox("Unable to find Sony Remote Camera Control or Imaging Edge", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
			return;
		}
		else
		{
			const int ButtonId = 0x3E9;
			//CWnd* hWndButton = rccwin->GetDlgItem(ButtonId);
			CWnd* hWndButton = rccwin->GetDescendantWindow(ButtonId);
			if (hWndButton==NULL)
			{
				this->MessageBox("Unable to find button on Sony Imaging Edge", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
				return;
			}
		}
	}
	else
	{
		const int ButtonId = 0x3EE;
		//CWnd* hWndButton = rccwin->GetDlgItem(ButtonId);
		CWnd* hWndButton = rccwin->GetDescendantWindow(ButtonId);
		if (hWndButton==NULL)
		{
			//If there's a dialog button then press it
			const int OkButtonId = 0x2;
			CWnd* hWndOkButton = rccwin->GetDescendantWindow(OkButtonId);
			if (hWndOkButton!=NULL)
			{
				int wParam = NULL;
				int lParam = NULL;
				BOOL success = hWndOkButton->SendMessage(WM_LBUTTONDOWN, wParam, lParam);
				success = hWndOkButton->SendMessage(WM_LBUTTONUP, wParam, lParam);
				Sleep(500);
			}
		}
		
		rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
		hWndButton = rccwin->GetDescendantWindow(ButtonId);
		if (hWndButton==NULL)
		{
			//If there's a dialog button then press it
			const int OkButtonId = 0x2;
			CWnd* hWndOkButton = rccwin->GetDescendantWindow(OkButtonId);
			if (hWndOkButton!=NULL)
			{
				int wParam = NULL;
				int lParam = NULL;
				BOOL success = hWndOkButton->SendMessage(WM_LBUTTONDOWN, wParam, lParam);
				success = hWndOkButton->SendMessage(WM_LBUTTONUP, wParam, lParam);
				Sleep(500);
			}
		}
		
		rccwin = CWnd::FindWindow(lpClassName,lpWindowName);
		hWndButton = rccwin->GetDescendantWindow(ButtonId);
		if (hWndButton==NULL)
		{
			this->MessageBox("Unable to find button on Sony Remote Camera Control", "MarksAcquisition Error", MB_ICONINFORMATION | MB_OK);
			return;
		}
	}

	CWnd* pWnd;
	pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
	pWnd->SetWindowText("Beginning acquisition ...");

	if (!UpdateData()) //Updates and checks member variables
		return;

	mStatus = 0;
	SetTimer(99, 1000, NULL);  //1000 is one second

	(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(false);
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(true);
	(this->GetDlgItem(IDC_BUTTON_PHDCONNECT))  ->EnableWindow(false);
}


void CMarksAcquisitionDlg::OnButtonAbort() 
{
	KillTimer(99);
	if (!mPhdConnected)
		(this->GetDlgItem(IDC_BUTTON_PHDCONNECT))  ->EnableWindow(true);
		
	
	BOOL ret = PostShutterButtonMessage(WM_LBUTTONUP);

	CWnd* pWnd;
	pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
	pWnd =  this->GetDlgItem(IDC_STATIC_EXPINFO);
	pWnd->SetWindowText("Acquisition aborted");
	
	if(mPhdConnected)
	{
		//CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
		pWnd->SetWindowText("Guiding");
	}
	(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);
	
}

void CMarksAcquisitionDlg::OnButtonPhdconnect() 
{
	CWnd* pWnd;
	pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
	pWnd->SetWindowText("Connecting ...");
	(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(false);
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);

		
		//CSocket mSockClient;
	BOOL ret = mSockClient.Create( );
	if (ret == false)
	{
		//TODO GetLastError
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
		pWnd->SetWindowText("Failed to create socket client");
	(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(true);
		return;
	}

	//ret = mSockClient.Connect("127.0.0.1.",4300);  //v1.0 had the trailing dot 
	ret = mSockClient.Connect("127.0.0.1",4300);
	if (ret == false)
	{
		//TODO GetLastError
		CWnd* pWnd;
		pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
		pWnd->SetWindowText("Failed to connect to PHD");
		(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
		(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(true);
		return;
	}

	mPhdConnected = true;
	pWnd =  this->GetDlgItem(IDC_STATIC_PHDINFO);
	pWnd->SetWindowText("Connected to PHD");

	(this->GetDlgItem(IDC_BUTTON_PHDCONNECT))  ->EnableWindow(false);
	(this->GetDlgItem(IDC_BUTTON_START))  ->EnableWindow(true);
	(this->GetDlgItem(IDC_BUTTON_ABORT))  ->EnableWindow(false);

}


void CMarksAcquisitionDlg::OnChangeEditExplength() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditInterframe() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditPhddithertimeout() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}
void CMarksAcquisitionDlg::OnChangeEditNumexp() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditPhdditheramount() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditPhdsettleaccuracy() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditDitherExposures() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}

void CMarksAcquisitionDlg::OnChangeEditPresettle() 
{
	// TODO: If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.
	
	// TODO: Add your control notification handler code here
	if (!UpdateData()) //Updates and checks member variables
		return;
	WriteIni();
	
}
