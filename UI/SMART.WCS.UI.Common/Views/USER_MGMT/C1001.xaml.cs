using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.HANJINE.Common.Popup;
using SMART.WCS.UI.COMMON.DataMembers.C1001;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.Common.Views.SYS_MGMT
{
    /// <summary>
    /// 사용자 관리 및 사용자 별 담당 설비 관리
    /// </summary>
    public partial class C1001 : UserControl, TabCloseInterface
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion
        #endregion

        public TextBlock g_ctrlTextBlock = new TextBlock();

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자 
        public C1001()
        {
            InitializeComponent();
        }

        public C1001(bool _isFlag)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">메뉴 네비게이션 정보</param>
        public C1001(List<string> _liMenuNavigation)
        {
            InitializeComponent();

            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                this.RoleCD_ComboBox = this.BaseClass.CenterCD;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1001), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void IsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TableView view = source as TableView;
                view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region > 사용자 관리
        public static readonly DependencyProperty UserMgntListProperty
            = DependencyProperty.Register("UserMgntList", typeof(ObservableCollection<UserMgnt>), typeof(C1001)
                , new PropertyMetadata(new ObservableCollection<UserMgnt>()));

        public ObservableCollection<UserMgnt> UserMgntList
        {
            get { return (ObservableCollection<UserMgnt>)GetValue(UserMgntListProperty); }
            set { SetValue(UserMgntListProperty, value); }
        }
        #endregion

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1001), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        public string RoleCD_ComboBox { get; set; }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            // 콤보박스 설정
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드 버튼 클릭 이벤트
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 비밀번호 초기화 클릭 이벤트
            this.btnPwdInit.PreviewMouseLeftButtonUp += BtnPwdInit_PreviewMouseLeftButtonUp;

            // 그리드 Row추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 그리드 추가Row 삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText()
        {
            var strResource                 = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
            var iTotalRowCount              = (this.gridMaster.ItemsSource as ICollection).Count;           // 전체 데이터 수
            this.GridRowCount               = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return this.CheckModifyData();
        }
        #endregion

        #region >> CheckModifyData - 각 탭의 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue              = true;
            string strMessage           = string.Empty;
            //string strSelectedName      = string.Empty;

            if (this.UserMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                strMessage          = this.BaseClass.GetResourceValue("ERR_EXISTS_NO_SAVE");
                bRtnValue           = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;

                if (bRtnValue == true)
                {
                    this.UserMgntList.Clear();
                }
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 Row 수정 여부 
        /// <summary>
        /// 그리드 Row 수정 여부 
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            bool isRtnValue = true;
            string strMessage = string.Empty;
            int iCheckedCount = 0;

            iCheckedCount = this.UserMgntList.Where(p => p.IsSelected == true).Count();

            if (iCheckedCount == 0)
            {
                // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NO_SELECT");

                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다.
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            //this.UserMgntList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            //{
            //    this.UserMgntList.Remove(p);
            //});

            var liUserMgnt = this.UserMgntList.Where(p => p.IsSelected == true).ToList();

            if (liUserMgnt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }

            if (liUserMgnt.Where(w => w.IsNew == false).Count() > 0)
            {
                this.BaseClass.MsgQuestion("ASK_DEL_DB");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) return;
            }

            //liEquipmentMgnt.ForEach(p => EquipmentMgntList.Remove(p));
            liUserMgnt.ForEach(p => {
                if (p.IsNew == false)
                {
                    p.USE_YN_CHECKED = false;

                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        this.InsertSP_USER_INS2(da, p);
                    }
                }

                UserMgntList.Remove(p);
            });
        }
        #endregion
        #endregion

        #region > Validation - 유효성 검사 
        #region >> ValidationSearchUser - 사용자 관리 리스트 조회시 조건 유효성 체크
        /// <summary>
        /// 사용자 관리 리스트 조회시 조건 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool ValidationSearchUser()
        {
            if (this.txtUserID.Text.Trim().Length == 0)
            {
                // ERR_EMPTY_USER_ID - 사용자 ID를 입력하세요.
                this.BaseClass.MsgError("ERR_EMPTY_USER_ID");
                return false;
            }

            if (this.txtUserNM.Text.Trim().Length == 0)
            {
                // ERR_EMPTY_USER_NM - 사용자명을 입력하세요.
                this.BaseClass.MsgError("ERR_EMPTY_USER_NM");
                return false;
            }

            return true;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_USER_LIST_INQ - 사용자 관리 리스트 조회
        /// <summary>
        /// 사용자 관리 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task GetSP_USER_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "UI_USER_MST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();

            var strMgmtUserID   = this.txtUserID.Text.Trim();                               // 사용자 ID
            var strUserNM       = this.txtUserNM.Text.Trim();                               // 사용자명
            var strUseYN        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("USER_ID",     strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("USER_NM",     strUserNM);         // 사용자명
            dicInputParam.Add("USE_YN",      strUseYN);         // 사용여부
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            #region > 사용자 관리 
            if (dsRtnValue == null) { return; }
                
            var strErrCode          = string.Empty;     // 오류 코드
            var strErrMsg           = string.Empty;     // 오류 메세지

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
            {
                // 정상 처리된 경우
                this.UserMgntList = new ObservableCollection<UserMgnt>();
                this.UserMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
            }
            else
            {
                // 오류가 발생한 경우
                this.UserMgntList.ToObservableCollection(null);
                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
            }

            this.gridMaster.ItemsSource = this.UserMgntList;

            if (this.UserMgntList.Count > 0)
            {
                this.BaseClass.SetGridRowAddFocuse(this.gridMaster, 0);
            }

            this.SetResultText();
            #endregion
        }
        #endregion

        private bool InsertSP_USER_INS2(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "UI_USER_MST_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strMgmtUserID       = _item.USER_ID;                                    // 사용자 ID
            var strUserNM           = _item.USER_NM;                                    // 사용자명
            var strRoleCD           = _item.ROLE_CD;                                    // 권한코드
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";         // 사용여부
            var strUserID           = this.BaseClass.UserID;                            // 사용자 ID
            var strInsType          = _item.IsNew ? "I" : _item.IsUpdate ? "U" : "";
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("USER_ID",        strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("ROLE_CD",        strRoleCD);         // 권한코드
            dicInputParam.Add("USER_NM",        strUserNM);         // 사용자명
            dicInputParam.Add("USE_YN",         strUseYN);          // 사용여부
            dicInputParam.Add("L_USER_ID",      strUserID);         // 사용자 ID
            dicInputParam.Add("INS_TYPE",       strInsType);        // 
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            #endregion

            if(dicRtnValue["RTN_VAL"].ToString().Equals("0") == false)
            {
                var strMessage = dicRtnValue["RTN_MSG"].ToString();
                this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                isRtnValue = false;
            }

            return isRtnValue;
        }

        private bool SP_USER_MST_PASS_INT(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "UI_USER_MST_PASS_INT";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strMgmtUserID = _item.USER_ID;                              // 사용자 ID
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("USER_ID", strMgmtUserID);                    // 사용자 ID
            dicInputParam.Add("L_USER_ID", strUserID);                      // 사용자 ID
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            #endregion

            if (dicRtnValue["RTN_VAL"].ToString().Equals("0") == false)
            {
                var strMessage = dicRtnValue["RTN_MSG"].ToString();
                this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //using (ExcelUpload popup = new ExcelUpload(SMART.WCS.HANJINE.Common.Enum.EnumClass.ExcelUploadKind.ORD_INFO))
                //{
                //    popup.ShowDialog();
                //}

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 사용자 관리 리스트 조회
                await this.GetSP_USER_LIST_INQ();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_EXCEL_DOWNLOAD - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                List<TableView> tv = new List<TableView>();
                tv.Add(this.MasterGrid);
                this.BaseClass.GetExcelDownload(tv);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 저장 버튼 클릭 이벤트
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 Row 수정 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;
                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                // ERR_NOT_SELECT - {0}이(가) 선택되지 않았습니다.
                string strSelectMessage = this.BaseClass.GetResourceValue("ERR_NOT_SELECT", BaseEnumClass.ResourceType.MESSAGE);

                this.UserMgntList.ForEach(p => p.ClearError());

                foreach (var item in this.UserMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.USER_ID) == true)
                        {
                            item.CellError("USER_ID", string.Format(strInputMessage, this.BaseClass.GetResourceValue("USER_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.USER_NM) == true)
                        {
                            item.CellError("USER_NM", string.Format(strInputMessage, this.BaseClass.GetResourceValue("USER_NM")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.ROLE_CD) == true)
                        {
                            item.CellError("ROLE_CD", string.Format(strSelectMessage, this.BaseClass.GetResourceValue("ROLE_CD")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.UserMgntList.Where(p => p.IsSelected == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = this.InsertSP_USER_INS2(da, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;
                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 사용자 리스트를 재조회한다.
                            this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch 
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        throw;
                    }
                    finally
                    {
                        if (da.TransactionState_MSSQL == BaseEnumClass.TransactionState_MSSQL.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 비밀번호 초기화 버튼 클릭 이벤트
        /// <summary>
        /// 비밀번호 초기화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPwdInit_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 Row 수정 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_PWD_CHG - 비밀번호를 재설정하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_PWD_CHG");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.UserMgntList.ForEach(p => p.ClearError());

                var liSelectedRowData = this.UserMgntList.Where(p => p.IsSelected == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = this.SP_USER_MST_PASS_INT(da, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;
                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 사용자 리스트를 재조회한다.
                            this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        throw;
                    }
                    finally
                    {
                        if (da.TransactionState_MSSQL == BaseEnumClass.TransactionState_MSSQL.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 Row 추가 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 Row 추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var newItme = new UserMgnt
                {
                    USER_ID                 = string.Empty
                    ,   USER_NM             = string.Empty
                    ,   ROLE_CD             = string.Empty
                    ,   USE_YN              = "Y"
                    ,   PWD_INIT_YN         = "Y"
                    ,   IsSelected          = true
                    ,   IsNew               = true
                };

                this.UserMgntList.Add(newItme);
                this.gridMaster.Focus();
                this.gridMaster.CurrentColumn            = this.gridMaster.Columns.First();
                this.gridMaster.View.FocusedRowHandle    = this.UserMgntList.Count - 1;

                this.UserMgntList[this.UserMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
                this.UserMgntList[this.UserMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

                this.BaseClass.SetGridRowAddFocuse(this.gridMaster, this.UserMgntList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CheckGridRowSelected() == false) { return; }

                // 행 추가된 그리드의 Row 중 선택된 Row를 삭제한다.
                this.DeleteGridRowItem();

                this.BaseClass.SetGridRowAddFocuse(this.gridMaster, this.UserMgntList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
        
        #region > 그리드 관련 이벤트
        #region >> 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view        = (sender as GridControl).View as TableView;
                var hitInfo     = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                if (hitInfo.InRowCell)
                {
                    // 그리드 내 체크박스 선택 로직
                    switch (hitInfo.Column.FieldName)
                    {
                        case "USE_YN":
                        //case "PWD_INIT_YN":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }

                            int iCurrentRowIndex = this.BaseClass.GetCurrentGridControlRowIndex(this.MasterGrid);
                            if (this.UserMgntList[iCurrentRowIndex].IsSelected == false) { }
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        /// <summary>
        /// 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                TableView tv = sender as TableView;
                UserMgnt dataMember = tv.Grid.CurrentItem as UserMgnt;
                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "USER_ID":
                        if (dataMember.IsNew == false)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                    //case "INIT_YN":
                    //    if (dataMember.IsNew == true)
                    //    {
                    //        if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                    //        e.Cancel = true;
                    //    }
                    //    break;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            try
            {
                //this.TreeControlRefreshEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
    }
}
