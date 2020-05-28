using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Views;
using SMART.WCS.HANJINE.Common.Popup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SMART.WCS.Main
{
    /// <summary>
    /// SMART WCS 로그인 화면
    /// </summary>
    public partial class Login : Window, IDisposable
    {
        #region ▩ 전역변수
        #region > 상수 선언
        private readonly string NORMAL_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton.png";
        private readonly string HOVER_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton_hover.png";
        private readonly string CLICK_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton.png";
        #endregion
        /// <summary>
        /// Base Class
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 데이터테이블 : 데이터베이스 연결 문자열 정보
        /// </summary>
        DataTable g_dtDatabaseConnectionInfo = new DataTable();

        /// <summary>
        /// App.config에 설정된 DB 접속 타입값
        /// </summary>
        private string g_strConfigDBConnectType = string.Empty;

        /// <summary>
        /// 신규 비밀번호 변경 성공 여부 - 성공("Y")=>메인화면, 실패 및 거부("N")=>종료
        /// </summary>
        private bool g_bInitPasswordSaveYN = false;
        #endregion

        #region ▩ 생성자
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 로그인 생성자
        /// </summary>
        /// <param name="_dtDatabaseConnectionInfo">데이터베이스 연결정보</param>
        public Login(DataTable _dtDatabaseConnectionInfo)
        {
            try
            {
                InitializeComponent();

                // 컨트롤 값 초기화
                this.InitValue();

                // 이벤트 초기화
                this.InitEvent();

                // App.config에 설정된 DB 접속 타입값
                this.g_strConfigDBConnectType = this.BaseClass.GetAppSettings("DBConnectType_DEV_REAL").Equals("DEV") == true ? "DEV" : "REAL";

                // 로그인 버튼 클릭 시 센터별 데이터베이스 연결 문자열을 가져오기 위해 데이터테이블을 전역변수에 저장한다.
                this.g_dtDatabaseConnectionInfo = _dtDatabaseConnectionInfo;

                // 센터 콤보박스 설정
                this.BindingComboBoxCenter();

                // 컨트롤 초기화
                this.InitControl();

                if (this.txtUserID.Text.Trim().Length > 0)
                {
                    this.txtPwd.Focus();
                }
                else
                {
                    this.txtUserID.Focus();
                }
            }
            catch(Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitValue - 컨트롤 데이터 초기화
        /// <summary>
        /// 컨트롤 데이터 초기화
        /// </summary>
        private void InitValue()
        {
            try
            {
                this.lblLogin.Text              = this.BaseClass.GetResourceValue("LOG_IN");
                this.txtUserID.Text             = this.BaseClass.LoginUserID.Trim();
                this.chkRememberID.IsChecked    = this.BaseClass.RememberChecked;
            }
            catch { throw; }
        }
        #endregion

        #region >> InitControl - 컨트롤 초기화
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                // 언어 콤보박스 설정
                this.BaseClass.BindingFirstComboBox(this.cboLang, "LANG");

                if (this.cboLang.Items.Count > 0)
                {
                    this.cboLang.SelectedIndex = 0;
                }

                // 첫번째 Row 선택이 Defult가 아닌 경우 아래 구분 수행
                var liVar = this.cboLang.ItemsSource as List<ComboBoxInfo>;
                if (liVar.Count > 0)
                {
                    this.cboLang.SelectedIndex = 0;
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.imgLoginBtn.PreviewMouseLeftButtonUp += ImgLoginBtn_PreviewMouseLeftButtonUp;
                this.imgLoginBtn.MouseLeave += ImgLoginBtn_MouseLeave;
                this.imgLoginBtn.MouseEnter += ImgLoginBtn_MouseEnter;
                this.imgLoginBtn.MouseUp += ImgLoginBtn_MouseLeave;

                this.lblLogin.PreviewMouseLeftButtonUp += ImgLoginBtn_PreviewMouseLeftButtonUp;
                this.lblLogin.MouseLeave += ImgLoginBtn_MouseLeave;
                this.lblLogin.MouseEnter += ImgLoginBtn_MouseEnter;
                this.lblLogin.MouseUp += ImgLoginBtn_MouseLeave;

                this.txtPwd.KeyDown += TxtPwd_KeyDown;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > BindingComboBoxCenter - 콤보박스 바인딩 (센터코드) - 공통 콤보박스 사용하지 않음
        /// <summary>
        /// 콤보박스 바인딩 (센터코드) - 공통 콤보박스 사용하지 않음
        /// </summary>
        private void BindingComboBoxCenter()
        {
            #region 중복 센터 정보를 필터링 하기 위해 GroupBy 처리한다.
            var query = from p in this.g_dtDatabaseConnectionInfo.AsEnumerable()
                        group p by new
                        {
                            CNTR_CD         = p.Field<string>("CNTR_CD"),
                            CNTR_NM         = p.Field<string>("CNTR_NM"),
                            DB_TYPE         = p.Field<string>("DB_CONN_TYPE")
                        } into q
                        where q.Key.DB_TYPE.Equals(this.g_strConfigDBConnectType)
                        select new
                        {
                            CODE            = q.Key.CNTR_CD,
                            NAME            = q.Key.CNTR_NM,
                            DB_CONN_TYPE    = q.Key.DB_TYPE

                        };
            #endregion

            // 콤보박스 바인딩을 위해 데이터를 저장하기 위한 데이터테이블
            DataTable dtNewTable = null;
            // 데이터테이블 스키마를 정의한다. (공통코드 형식)
            dtNewTable = this.BaseClass.CreateDataTableSchema(dtNewTable, BaseEnumClass.CreateTableSchemaKind.COMMON_CODE);

            // GroupBy 처리한 데이터를 데이터 테이블에 저장한다.
            foreach (var itemCenterInfo in query)
            {
                var aaa = itemCenterInfo.CODE;
                var bbb = itemCenterInfo.NAME;

                DataRow drNewRow = dtNewTable.NewRow();
                drNewRow["CODE"] = itemCenterInfo.CODE;
                drNewRow["NAME"] = itemCenterInfo.NAME;
                dtNewTable.Rows.Add(drNewRow);
            }

            // 데이터테이블에 저장한 센터정보를 콤보박스 바인딩을 위해 List형식으로 저장한다.
            var liComboBoxInfo = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtNewTable);
            // 센터 정보를 콤보박스에 바인딩한다.
            this.cboCenter.ItemsSource = liComboBoxInfo;

            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (liComboBoxInfo.Count > 0)
            {
                if (this.BaseClass.LoginCenterCD.Length > 0)
                {
                    this.cboCenter.SelectedIndex = this.BaseClass.MatchComboIndex(this.cboCenter, this.BaseClass.LoginCenterCD);
                }
                else
                {
                    this.cboCenter.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region > LoginProcess - 로그인 프로세스
        /// <summary>
        /// 로그인 프로세스
        /// </summary>
        /// <param name="_strUserID">사용자 ID</param>
        private void LoginProcess(string _strUserID)
        {
            try
            {
                #region + 로그인 성공
                if (this.chkRememberID.IsChecked == true)
                {
                    this.BaseClass.LoginUserID = _strUserID;
                }
                else
                {
                    this.BaseClass.LoginUserID = string.Empty;
                }

                var dsLoginInfo         = this.GetMenuList();
                if (dsLoginInfo.Tables[0].Rows.Count == 0)
                {
                    this.BaseClass.MsgError("ERR_NOT_MENU_DATA");
                    return;
                }
                else
                {
                    this.BaseClass.RememberChecked = this.chkRememberID.IsChecked == true ? true : false;

                    this.CallMainWindow(dsLoginInfo);

                }
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region > CallMainWindow - 메인 화면을 호출한다.
        /// <summary>
        /// 메인 화면을 호출한다.
        /// </summary>
        /// <param name="_dsLoginInfo">로그인 정보</param>
        private void CallMainWindow(DataSet _dsLoginInfo)
        {
            try
            {
                // 전역으로 사용하는 사용자 ID
                this.BaseClass.UserID = this.txtUserID.Text.Trim();

                if (this.chkRememberID.IsChecked == true)
                {   
                    // 로그인 창 오픈시 RememberID가 체크된 경우 불러오는 사용자 ID
                    this.BaseClass.LoginUserID  = this.txtUserID.Text.Trim();
                }
                else
                {
                    this.BaseClass.LoginUserID  = string.Empty;
                }

                // 선택한 센터코드를 사용자 정보에 저장한다.
                this.BaseClass.LoginCenterCD   = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);

                MainWindow frmMain = new MainWindow(_dsLoginInfo);
                frmMain.Show();

                this.Close();
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 관련
        #region >> [조회] GetMenuList - 메뉴 정보를 조회한다.
        /// <summary>
        /// 메뉴 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataSet GetMenuList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "UI_MENU_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                #endregion

                #region Input 파라메터
                //var strCenterCD     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                var strUserID       = this.txtUserID.Text.Trim();

                dicInputParam.Add("USER_ID",      strUserID);         // 사용자 ID
                #endregion

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> [조회] GetNoticeList - 공지사항 정보를 조회한다.
        /// <summary>
        /// 공지사항 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        /// 
        private DataSet GetNoticeList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "CSP_C1000_SP_NOTICE_TALLY_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                #endregion

                #region Input 파라메터
                var strCenterCD         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                var strUserID           = this.txtUserID.Text.Trim();
                var strDateTime         = DateTime.Now.ToString("yyyyMMdd");
                #endregion

                dicInputParam.Add("P_CNTR_CD",  strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID",  strUserID);         // 사용자 ID
                dicInputParam.Add("P_DATE",     strDateTime);       // 오늘날짜 (yyyymmdd)

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }
                return dsRtnValue;
            }
            catch { throw; }

        }
        #endregion

        #region >> [조회] GetSP_LOGIN_LIST_INQ - 로그인
        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="_strUserID">사용자 ID</param>
        /// <param name="_strPwd">비밀번호</param>
        /// <returns></returns>
        private int GetSP_LOGIN_LIST_INQ(string _strUserID, string _strPwd)
        {
            try
            {
                int iRtnValue   = -1;

                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "UI_LOGIN_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "RTN_VAL", "RTN_MSG" };
                #endregion

                #region Input 파라메터
                dicInputParam.Add("USER_ID",          _strUserID);                                // 사용자 ID
                dicInputParam.Add("PWD",              _strPwd);                                   // 비밀번호
                #endregion

                var strErrCode          = string.Empty;
                var strErrMsg           = string.Empty;

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    this.BaseClass.MsgError("", BaseEnumClass.CodeMessage.MESSAGE);
                    iRtnValue = 99;
                }
                else
                {
                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue.Tables[0], ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                    {
                        #region + 로그인 성공
                        iRtnValue = 0;
                        #endregion
                    }
                    else
                    {
                        switch (strErrCode)
                        {
                            case "1":
                                #region + 비밀번호 재설정
                                // INFO_PASSWORD_RESETTING_OPEN_WINDOW - 비밀번호 재설정이 필요합니다.|창이 오픈되면 비밀번호를 재설정해주세요.
                                this.BaseClass.MsgInfo("INFO_PASSWORD_RESETTING_OPEN_WINDOW");
                                iRtnValue = 1;
                                #endregion
                                break;

                            case "2":
                                #region + 사용자 정보 없음
                                iRtnValue = 2;
                                this.BaseClass.MsgError("ERR_NOT_USER_INFO");
                                #endregion
                                break;
                            case "3":
                                #region + 비밀번호 일치 여부
                                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                                iRtnValue = 3;
                                #endregion
                                break;
                            case "9":
                                #region 연결 IP 체크
                                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                                iRtnValue = 9;
                                #endregion
                                break;
                        }
                    }
                }

                return iRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 로그인 버튼 관련 이벤트
        #region >> 버튼 이미지 마우스 후버 관련 이벤트
        private void ImgLoginBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.imgLoginBtn.Source = new BitmapImage(new Uri(HOVER_IMAGE_PATH));
        }

        private void ImgLoginBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.imgLoginBtn.Source = new BitmapImage(new Uri(NORMAL_IMAGE_PATH));
        }
        #endregion

        #region >> 로그인 버튼 이미지 클릭 이벤트
        /// <summary>
        /// 로그인 버튼 이미지 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgLoginBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.imgLoginBtn.Source = new BitmapImage(new Uri(CLICK_IMAGE_PATH));

            try
            {
                var strUserID           = this.txtUserID.Text.Trim();   // 사용자 ID
                var strPwd              = this.txtPwd.Text.Trim();      // 비밀번호

                if (strUserID.Length == 0)
                {
                    this.BaseClass.MsgError("ERR_EMPTY_USER_ID");
                    this.txtUserID.Focus();
                    return;
                }

                if (strPwd.Length == 0)
                {
                    this.BaseClass.MsgError("ERR_EMPTY_PWD_NM");
                    this.txtPwd.Focus();
                    return;
                }

                var query = this.g_dtDatabaseConnectionInfo.AsEnumerable().Where(p => p.Field<string>("DB_CONN_TYPE").Equals(this.g_strConfigDBConnectType)).FirstOrDefault();

                if (query == null)
                {
                    // ERR_NOT_EXIST_DATABASE_CONNECT_STRING - 데이터베이스 연결 문자열이 존재하지 않습니다.
                    this.BaseClass.MsgError("ERR_NOT_EXIST_DATABASE_CONNECT_STRING");
                    return;
                }
                else
                {
                    this.BaseClass.DatabaseConnectionString_ORACLE = query.Field<string>("ORCL_CONN_STR");  // 오라클 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MSSQL = query.Field<string>("MS_CONN_STR");    // MS-SQL 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MariaDB = query.Field<string>("MR_CONN_STR");    // MariaDB 연결 문자열
                }

                // 로그인 처리
                var iResult = this.GetSP_LOGIN_LIST_INQ(strUserID, strPwd);

                if (iResult == 0)
                {
                    this.LoginProcess(strUserID);
                }
                else if (iResult == 1)
                {
                    #region + 비밀번호 재설정
                    var strCenterCD = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);

                    using (ResetPassword frmPopup = new ResetPassword(strUserID, strCenterCD))
                    {
                        frmPopup.PasswordChangeResult += FrmPopup_PasswordChangeResult;
                        frmPopup.ShowDialog();
                    }

                    if (this.g_bInitPasswordSaveYN == true)
                    {
                        this.LoginProcess(strUserID);
                    }
                    else
                    {
                        this.Close();
                    }
                    #endregion
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 비밀번호 재설정 팝업 결과값 수신 이벤트 (Delegate)
        /// <summary>
        /// 비밀번호 재설정 팝업 결과값 수신 이벤트 (Delegate)
        /// </summary>
        /// <param name="_bResultYN"></param>
        private void FrmPopup_PasswordChangeResult(bool _bResultYN)
        {
            try
            {
                this.g_bInitPasswordSaveYN = _bResultYN;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 비밀번호 TextBox Enter키 입력 이벤트
        /// <summary>
        /// 비밀번호 TextBox Enter키 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    this.ImgLoginBtn_PreviewMouseLeftButtonUp(null, null);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~Login()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
