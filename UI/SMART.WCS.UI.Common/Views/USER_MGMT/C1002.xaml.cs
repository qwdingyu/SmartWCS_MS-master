using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.COMMON.DataMembers.C1002;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 권한관리 및 권한별 메뉴 리스트 관리
    /// </summary>
    public partial class C1002 : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자 
        public C1002()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public C1002(List<string> _liMenuNavigation)
        {
            InitializeComponent();

            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        #region >> 그리드 컨트롤
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1002), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
        #endregion

        #region > 권한 관리
        public static readonly DependencyProperty RoleMgntListProperty
            = DependencyProperty.Register("RoleMgntList", typeof(ObservableCollection<RoleMgnt>), typeof(C1002)
                , new PropertyMetadata(new ObservableCollection<RoleMgnt>()));

        public ObservableCollection<RoleMgnt> RoleMgntList
        {
            get { return (ObservableCollection<RoleMgnt>)GetValue(RoleMgntListProperty); }
            set { SetValue(RoleMgntListProperty, value); }
        }
        #endregion

        #region > 권한별 메뉴 리스트
        public static readonly DependencyProperty MenuListByRoleListProperty
            = DependencyProperty.Register("MenuListByRoleList", typeof(ObservableCollection<MenuListByRole>), typeof(C1002)
                , new PropertyMetadata(new ObservableCollection<MenuListByRole>()));

        public ObservableCollection<MenuListByRole> MenuListByRoleList
        {
            get { return (ObservableCollection<MenuListByRole>)GetValue(MenuListByRoleListProperty); }
            set { SetValue(MenuListByRoleListProperty, value); }
        }
        #endregion

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1002), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion
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
            #region + 화면 이벤트
            this.Loaded += C1002_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSEARCH_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSAVE.PreviewMouseLeftButtonUp += BtnSAVE_PreviewMouseLeftButtonUp;

            // 그리드 Row추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 그리드 추가Row 삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // 그리드 클릭 이벤트 (상세조회 - 권한별 메뉴 리스트 조회)
            this.gridLeft_RoleList.PreviewMouseLeftButtonUp += GridLeft_RoleList_PreviewMouseLeftButtonUp;

            #endregion

            #region + 트리 리스트 컨트롤 이벤트
            // 트리 리스트 컨트롤 클릭 이벤트(권한 콤보박스 선택시 하위 콤보박스까지 변경하는 로직)
            this.treeListControl.PreviewMouseLeftButtonUp += TreeListControl_PreviewMouseLeftButtonUp;

            this.treeListView.CellValueChanged += TreeListView_CellValueChanged;

            this.treeListView.CellValueChanging += TreeListView_CellValueChanging;
            #endregion
        }

        private void TreeListView_CellValueChanging(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCellValueChangedEventArgs e)
        {
            try
            {
                var strSelectedTreeID   = (this.treeListControl.SelectedItem as MenuListByRole).TREE_ID;    // 트리 ID
                var liFilterData        = this.MenuListByRoleList.Where(p => p.PARENT_ID.Contains(strSelectedTreeID) == true).ToList();

                if (e.Column.FieldName.Equals("ROLE_MENU_CD") == true)
                {   
                    var strOldValue     = e.OldValue.ToString();    // 이전 데이터
                    var strNewValue     = e.Value.ToString();       // 변경 데이터

                    if (liFilterData.Count > 0)
                    {
                        foreach (var item in liFilterData)
                        {
                            item.ROLE_MENU_CD           = strNewValue;
                            item.IsSelected             = true;
                            item.IsUpdate               = true;
                            item.BaseBackgroundBrush    = new SolidColorBrush(Colors.GhostWhite);
                        }
                    }

                    int iCurrentRowIndex                            = this.BaseClass.GetCurrentGridControlRowIndex(this.tvLeftGrid_RoleList);
                    this.RoleMgntList[iCurrentRowIndex].IsSelected  = true;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void C1002_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.g_isLoaded == true) { return; }

            this.BtnSEARCH_PreviewMouseLeftButtonUp(null, null);

            this.g_isLoaded = true;
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
            var strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                  // 텍스트 리소스
            var iTotalRowCount = (this.gridLeft_RoleList.ItemsSource as ICollection).Count;     // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";          // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                          // 건의 데이터가 조회되었습니다.
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
            bool isRtnValue     = true;

            isRtnValue = this.CheckModifyRoleMstData();

            if (isRtnValue == false)
            {
                isRtnValue = this.CheckModifyMenuListByRoleData();
            }

            return isRtnValue;
        }
        #endregion

        #region >> CheckModifyRoleMstData - 데이터 저장 여부를 체크한다. (권한 마스터 리스트)
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다. (권한 마스터 데이터)
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyRoleMstData()
        {
            bool bRtnValue = true;

            if (this.RoleMgntList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                // 저장되지 않은 데이터가 있습니다.|조회하시겠습니까?
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_INQ");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckModifyMenuListByRoleData - 데이터 저장 여부를 체크한다. (권한별 메뉴 리스트)
        /// <summary>
        /// 데이터 저장 여부를 체크한다. (권한별 메뉴 리스트)
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyMenuListByRoleData()
        {
            bool bRtnValue = true;

            if (this.MenuListByRoleList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 Row 수정 여부 (IsSelected 값으로 판단)
        /// <summary>
        /// 그리드 Row 수정 여부 - (IsSelected 값으로 판단)
        /// </summary>
        /// <param name="_enumScreenGridKind">그리드 종류</param>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            var isRtnValue          = true;
            var strMessage          = string.Empty;
            var iCheckedHrdCount    = this.RoleMgntList.Where(p => p.IsSelected == true).Count();

            if (iCheckedHrdCount == 0)
            {
                // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NO_SELECT");
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        //#region >> CheckGridRowChangeData - 그리드 컬럼값 수정 여부 (IsSelected, IsUpdate 값으로 판단)
        ///// <summary>
        ///// 그리드 컬럼값 수정 여부 (IsSelected, IsUpdate 값으로 판단)
        ///// </summary>
        ///// <returns></returns>
        //private bool CheckGridRowChangeData()
        //{
        //    var isRtnValue          = true;
        //    var strMessage          = string.Empty;
        //    var iCheckedDtlCount    = this.MenuListByRoleList.Where(p => p.IsSelected && p.IsUpdate).Count();

        //    if (iCheckedDtlCount == 0)
        //    {
        //        // ERR_NOT_CHANGE_DATA - 변경된 데이터가 없습니다.
        //        this.BaseClass.MsgError("ERR_NOT_CHANGE_DATA");
        //        isRtnValue = false;
        //    }

        //    return isRtnValue;
        //}
        //#endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다.
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            //this.RoleMgntList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            //{
            //    this.RoleMgntList.Remove(p);
            //});

            if (this.RoleMgntList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0 )
            {
                this.BaseClass.MsgQuestion("ASK_DEL_DB");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
            }

            this.RoleMgntList.Where(p => p.IsSelected == true).ToList().ForEach(p =>
            {
                if(p.IsNew !=  true)
                {
                    p.USE_YN_CHECKED = false;

                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        this.SaveSP_ROLE_HDR_INS(da, p);
                    }

                }

                this.RoleMgntList.Remove(p);
            });
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_ROLE_HDR_LIST_INQ - 권한 (헤더) 리스트를 조회한다.
        /// <summary>
        /// 권한 (헤더) 리스트를 조회한다.
        /// </summary>
        /// <returns></returns>
        private void GetSP_ROLE_HDR_LIST_INQ()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "UI_ROLE_MST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();

                var strRoleCD       = this.txtRoleCD.Text.Trim();                               // 권한코드
                var strRoleNM       = this.txtRoleNM.Text.Trim();                               // 권한명
                var strUseYN        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("ROLE_CD",          strRoleCD);         // 권한코드
                dicInputParam.Add("ROLE_NM",          strRoleNM);         // 권한명
                dicInputParam.Add("USE_YN",           strUseYN);          // 사용여부
                #endregion

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }
                #endregion

                #region ++ 권한관리
                if (dsRtnValue == null) { return; }
                
                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                {
                    // 정상 처리된 경우
                    this.RoleMgntList   = new ObservableCollection<RoleMgnt>();
                    this.RoleMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.RoleMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                this.gridLeft_RoleList.ItemsSource = this.RoleMgntList;

                this.SetResultText();
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSP_ROLE_DTL_LIST_INQ - 권한별 메뉴 리스트 조회
        /// <summary>
        /// 권한별 메뉴 리스트 조회
        /// </summary>
        /// <param name="_strRoleCD">권한 코드</param>
        /// <returns></returns>
        private void GetSP_ROLE_DTL_LIST_INQ(string _strRoleCD)
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "UI_ROLE_DTL_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();

                var strRoleCD       = _strRoleCD;                       // 권한코드
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("ROLE_CD",      strRoleCD);         // 권한코드
                #endregion

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }
                #endregion

                #region ++ 권한 별 메뉴 리스트
                if (dsRtnValue == null) { return; }
                
                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                {
                    // 정상 처리된 경우
                    this.MenuListByRoleList   = new ObservableCollection<MenuListByRole>();
                    this.MenuListByRoleList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.MenuListByRoleList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                this.treeListControl.ItemsSource = this.MenuListByRoleList;

                this.SetResultText();
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region >> InsertSP_ROLE_HDR_INS - 권한 마스터 신규 데이터 저장
        /// <summary>
        /// 권한 마스터 신규 데이터 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">신규 저장 대상 데이터</param>
        /// <returns></returns>
        private async Task<bool> SaveSP_ROLE_HDR_INS(BaseDataAccess _da, RoleMgnt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "UI_ROLE_MST_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strRoleCD           = _item.ROLE_CD;                                // 권한코드
            var strRoleNM           = _item.ROLE_NM;                                // 권한명
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";     // 사용여부
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            var strInsType          = _item.IsNew == true ? "I" : "U";              // Ins Type
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("ROLE_CD",      strRoleCD);     // 권한코드
            dicInputParam.Add("ROLE_NM",      strRoleNM);     // 권한명
            dicInputParam.Add("USE_YN",       strUseYN);      // 사용여부
            dicInputParam.Add("L_USER_ID",    strUserID);     // 사용자 ID
            dicInputParam.Add("INS_TYPE",     strInsType);    // Ins Type
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            }).ConfigureAwait(true);
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

        private async Task<bool> SaveSP_ROLE_DTL_SAVE(BaseDataAccess _da, MenuListByRole _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "UI_ROLE_DTL_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strRoleCD           = (this.gridLeft_RoleList.SelectedItem as RoleMgnt).ROLE_CD;    // 권한코드
            var strMenuID           = _item.MENU_ID;                                                // 메뉴 ID
            var strRoleMenuCD       = _item.ROLE_MENU_CD;                                           // 권한 타입
            var strUserID           = this.BaseClass.UserID;                                        // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("ROLE_CD",                strRoleCD);             // 권한코드
            dicInputParam.Add("MENU_ID",                strMenuID);             // 메뉴 ID
            dicInputParam.Add("ROLE_MENU_CD",           strRoleMenuCD);         // 권한 타입
            dicInputParam.Add("L_USER_ID",              strUserID);             // 사용자 ID
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            }).ConfigureAwait(true);
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
        private void BtnSEARCH_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 사용자 관리 리스트 조회
                this.GetSP_ROLE_HDR_LIST_INQ();

                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_RoleList, 0);

                
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                if (this.RoleMgntList.Count > 0)
                {
                    var liRoleList  = this.RoleMgntList.FirstOrDefault();

                    if (liRoleList.ROLE_CD.Length > 0)
                    {
                        this.GetSP_ROLE_DTL_LIST_INQ(liRoleList.ROLE_CD);
                        this.treeListView.ExpandAllNodes();
                    }
                }
                else
                {
                    this.treeListControl.ItemsSource = null;
                }

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
                tv.Add(this.tvLeftGrid_RoleList);
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
        private async void BtnSAVE_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                bool isRtnValue = false;

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                // ERR_NOT_SELECT - {0}이(가) 선택되지 않았습니다.
                string strSelectMessage = this.BaseClass.GetResourceValue("ERR_NOT_SELECT", BaseEnumClass.ResourceType.MESSAGE);

                #region 권한 관리 마스터 데이터
                this.RoleMgntList.ForEach(p => p.ClearError());

                foreach (var item in this.RoleMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.ROLE_CD) == true)
                        {
                            item.CellError("USER_ID", string.Format(strInputMessage, this.BaseClass.GetResourceValue("USER_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.ROLE_CD) == true)
                        {
                            item.CellError("ROLE_CD", string.Format(strSelectMessage, this.BaseClass.GetResourceValue("ROLE_CD")));
                            return;
                        }
                    }
                }
                #endregion

                // ASK_SAVE - 저장 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                #region 권한별 메뉴 리스트 데이터
                this.MenuListByRoleList.ForEach(p => p.ClearError());

                foreach (var item in this.MenuListByRoleList)
                {
                    if (item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.ROLE_MENU_CD) == true)
                        {
                            item.CellError("ROLE_MENU_CD", string.Format(strSelectMessage, this.BaseClass.GetResourceValue("ROLE_MENU_CD")));
                            return;
                        }
                    }
                }
                #endregion

                var liSelectedHrdData   = this.RoleMgntList.Where(p => p.IsSelected == true).ToList();
                var liUpdateDtlData     = this.MenuListByRoleList.Where(p => p.IsUpdate == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        #region 권한관리 마스터 데이터 저장
                        foreach (var item in liSelectedHrdData)
                        {
                            isRtnValue = await this.SaveSP_ROLE_HDR_INS(da, item);
                            if (isRtnValue == false) { break; }
                        }
                        #endregion

                        //if (isRtnValue == true)
                        //{
                            #region 권한별 메뉴 리스트
                            foreach (var item in liUpdateDtlData)
                            {
                                if (item.IsUpdate == true)
                                {
                                    isRtnValue = await this.SaveSP_ROLE_DTL_SAVE(da, item);
                                    if (isRtnValue == false) { break; }
                                }
                            }
                            #endregion
                        //}

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;
                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 사용자 리스트를 재조회한다.
                            this.BtnSEARCH_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        if (da.TransactionState_MSSQL == BaseEnumClass.TransactionState_MSSQL.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }
                        
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
                var newItme = new RoleMgnt
                {
                        ROLE_CD         = string.Empty
                    ,   ROLE_NM         = string.Empty
                    ,   USE_YN          = "Y"
                    ,   IsSelected      = true
                    ,   IsNew           = true
                };

                this.RoleMgntList.Add(newItme);
                this.gridLeft_RoleList.Focus();
                this.gridLeft_RoleList.CurrentColumn            = this.gridLeft_RoleList.Columns.First();
                this.gridLeft_RoleList.View.FocusedRowHandle    = this.RoleMgntList.Count - 1;

                this.RoleMgntList[this.RoleMgntList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
                this.RoleMgntList[this.RoleMgntList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);

                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_RoleList, this.RoleMgntList.Count - 1);
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

                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_RoleList, this.RoleMgntList.Count - 1);
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
        private void GridLeft_RoleList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
                if (hi.InRowCell)
                {
                    //if (hi.Column.FieldName.Equals("USE_YN") == false || hi.Column.FieldName.Equals("IP_MGMT_YN") =) { return; }

                    switch (hi.Column.FieldName)
                    {
                        case "ROLE_CD_REQ":
                        case "USE_YN":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }
                            break;
                    }
                }

                    // 권한 리스트 수정 여부를 확인한다.
                    int iUpdateRowCount = this.RoleMgntList.Where(p => p.IsSelected == true && p.IsUpdate == true).ToList().Count();
                    //if (iUpdateRowCount > 0)
                    //{
                    //    if (this.CheckModifyRoleMstData() == true)
                    //    {
                    //        this.GetSP_ROLE_HDR_LIST_INQ();
                    //    }
                    //}

                    iUpdateRowCount = 0;
                    // 권한별 메뉴 리스트 권한 정보 (콤보박스)의 수정 여부를 확인한다.
                    iUpdateRowCount = this.MenuListByRoleList.Where(p => p.IsUpdate == true).ToList().Count();
                    if (iUpdateRowCount > 0)
                    {
                        if (this.CheckModifyMenuListByRoleData() == false) { return; }
                    }

                    var strSelectedItemRoleCD = (this.gridLeft_RoleList.SelectedItem as RoleMgnt).ROLE_CD;  // 권한 코드
                    this.GetSP_ROLE_DTL_LIST_INQ(strSelectedItemRoleCD);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
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
                RoleMgnt dataMember = tv.Grid.CurrentItem as RoleMgnt;
                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "ROLE_CD":
                        if (dataMember.IsNew == false)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                    case "USE_YN":
                        if (dataMember.IsNew == true)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 트리 리스트 컨트롤 관련 이벤트
        #region >> 트리 리스트 컨트롤 클릭 이벤트 (권한 콤보박스 선택시 하위 콤보박스까지 변경하는 로직)
        /// <summary>
        /// 트리 리스트 컨트롤 클릭 이벤트 (권한 콤보박스 선택시 하위 콤보박스까지 변경하는 로직)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TreeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view        = (sender as TreeListControl).View as TreeListView;
                var hitInfo     = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                var strSelectedMenuID   = (this.treeListControl.SelectedItem as MenuListByRole).MENU_ID;        // 메뉴 ID
                var strSelectedTreeID   = (this.treeListControl.SelectedItem as MenuListByRole).TREE_ID;        // 트리 ID
                var strSelectedRoleType = (this.treeListControl.SelectedItem as MenuListByRole).ROLE_MENU_CD;   // 권한 타입
                var liFilterData        = this.MenuListByRoleList.Where(p => p.PARENT_ID.Equals(strSelectedMenuID) == true).ToList();

                if (hitInfo.InRowCell == true)
                {
                    switch (hitInfo.Column.FieldName)
                    {
                        case "ROLE_MENU_CD":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }
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

        #region >> 그리드 셀 데이터 변경 이벤트 - 권한별 메뉴 리스트 (권한 타입 변경)
        /// <summary>
        /// 그리드 셀 데이터 변경 이벤트 - 권한별 메뉴 리스트 (권한 타입 변경)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeListView_CellValueChanged(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCellValueChangedEventArgs e)
        {
            try
            {
                //var strSelectedTreeID   = (this.treeListControl.SelectedItem as MenuListByRole).TREE_ID;    // 트리 ID
                //var liFilterData        = this.MenuListByRoleList.Where(p => p.PARENT_ID.Contains(strSelectedTreeID) == true).ToList();

                //if (e.Column.FieldName.Equals("ROLE_MENU_CD") == true)
                //{   
                //    var strOldValue     = e.OldValue.ToString();    // 이전 데이터
                //    var strNewValue     = e.Value.ToString();       // 변경 데이터

                //    if (liFilterData.Count > 0)
                //    {
                //        foreach (var item in liFilterData)
                //        {
                //            item.ROLE_MENU_CD           = strNewValue;
                //            item.IsSelected             = true;
                //            item.IsUpdate               = true;
                //            item.BaseBackgroundBrush    = new SolidColorBrush(Colors.GhostWhite);
                //        }
                //    }
                //}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
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
