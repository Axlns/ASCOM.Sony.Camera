// MarksAcquisition.h : main header file for the MarksAcquisition application
//

#if !defined(AFX_MARKSACQUISITION_H__315F1F52_2825_4604_98AC_3B5B3EEE4616__INCLUDED_)
#define AFX_MARKSACQUISITION_H__315F1F52_2825_4604_98AC_3B5B3EEE4616__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

#include "MarksAcquisitionDlg.h"

/////////////////////////////////////////////////////////////////////////////
// CMarksAcquisitionApp:
// See MarksAcquisition.cpp for the implementation of this class
//

class CMarksAcquisitionApp : public CWinApp
{
public:
	CMarksAcquisitionApp();


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMarksAcquisitionApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CMarksAcquisitionApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()


};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MarksAcquisition_H__315F1F52_2825_4604_98AC_3B5B3EEE4616__INCLUDED_)
