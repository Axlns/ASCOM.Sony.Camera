// MarksAcquisitionDlg.h : header file
//

#if !defined(AFX_MARKSACQUISITION_H__FC0BC483_3A4C_43A1_8990_1C95138C8A5F__INCLUDED_)
#define AFX_MARKSACQUISITION_H__FC0BC483_3A4C_43A1_8990_1C95138C8A5F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CMarksAcquisitionDlg dialog

class CMarksAcquisitionDlg : public CDialog
{
// Construction
public:
	CMarksAcquisitionDlg(CWnd* pParent = NULL);	// standard constructor
	BOOL ExecuteSequence();
	void WriteIni();


// Dialog Data
	//{{AFX_DATA(CMarksAcquisitionDlg)
	enum { IDD = IDD_MARKSACQUISITION_DIALOG };
	int		mExpLength;
	int		mInterframe;
	int		mNumFrames;
	int		m_ComPort;
	int		m_PhdDitherAmount;
	double	m_PhdSettleAccuracy;
	int		m_PhdDitherTimeout;
	int		m_DitherExposures;
	int		m_PhdPreSettle;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMarksAcquisitionDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CMarksAcquisitionDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnChangeEditNumexp();
	afx_msg void OnButtonStart();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnButtonAbort();
	afx_msg void OnButtonPhdconnect();
	afx_msg void OnChangeEditExplength();
	afx_msg void OnChangeEditInterframe();
	afx_msg void OnChangeEditPhddithertimeout();
	afx_msg void OnChangeEditPhdditheramount();
	afx_msg void OnChangeEditPhdsettleaccuracy();
	afx_msg void OnChangeEditDitherExposures();
	afx_msg void OnChangeEditPresettle();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
//	int	mExpLen;
//	int mNumExps;
//	int mInter;
//	int mComPort;

	int mStatus;
	int mFrameCounter;
	int mSecondsCounter;
	int mDitherExposureCount;
	HANDLE m_hComm;
	BOOL mPhdConnected;
	BOOL mDitherStarted;
	CSocket mSockClient;
	CFont m_font;

	BOOL PostShutterButtonMessage(UINT message);

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MarksAcquisitionDLG_H__FC0BC483_3A4C_43A1_8990_1C95138C8A5F__INCLUDED_)
