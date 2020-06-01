using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using LGCNS.ezControl.Core;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.M1003;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.SORTER
{
    /// <summary>
    /// M1003.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class M1003 : UserControl, TabCloseInterface
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion

        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// Base Info 선언
        /// </summary>
        private BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 조회한 EQP_ID 저장
        /// </summary>
        private string SearchedEQPId = null;

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_IsLoaded = false;

        /// <summary>
        /// 화면 언로드 여부
        /// </summary>
        private bool g_IsUnLoaded = false;

        /// <summary>
        /// EzControl과 통신하기위한 Reference를 선언한다. 
        /// </summary>
        private CReference _reference = null;

        /// <summary>
        /// CJ때는 ElementNo를 DB에서 가져왔었는데 어차피 소터 하나인데 DB갔다오는게 안좋을 것 같아서 그냥 선언했음
        /// </summary>
        private int ElementNo = 1;
        #endregion

        #region ▩ 생성자
        public M1003()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public M1003(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)


                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                //this.InitComboBoxInfo();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(M1003), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 슈트 리스트
        public static readonly DependencyProperty ChuteMgmtListProperty
            = DependencyProperty.Register("ChuteMgmtList", typeof(ObservableCollection<ChuteMgmt>), typeof(M1003)
                , new PropertyMetadata(new ObservableCollection<ChuteMgmt>()));

        private ObservableCollection<ChuteMgmt> ChuteMgmtList
        {
            get { return (ObservableCollection<ChuteMgmt>)GetValue(ChuteMgmtListProperty); }
            set { SetValue(ChuteMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(M1003), new PropertyMetadata(string.Empty));

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
        #endregion

        #region ▩ 함수

        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "WSR", BaseClass.UserID, string.Empty };
            // 콤보박스 - 조회 (사용여부, 슈트 종류 코드, 위치 코드)
            this.BaseClass.BindingCommonComboBox(this.cboEqpId, "EQP_ID", commonParam_EQP_ID, true);   // 설비
            this.BaseClass.BindingCommonComboBox(this.cboChuteTypeCd, "CHUTE_TYPE_CD", null, true);       // 
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, false);

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_First_PreviewMouseLeftButtonUp;

            // 행 추가
            this.btnRowAdd_First.PreviewMouseLeftButtonUp += BtnRowAdd_First_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete_First.PreviewMouseLeftButtonUp += BtnRowDelete_First_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            this.tvMasterGrid.CellValueChanged += TvMasterGrid_CellValueChanged;
            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMaster.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 체크박스 선택 유효성 체크
        /// <summary>
        /// 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue = true;
                int iCheckedCount = 0;

                iCheckedCount = this.ChuteMgmtList.Where(p => p.IsSelected == true).Count();

                if (iCheckedCount == 0)
                {
                    bRtnValue = false;
                    BaseClass.MsgError("ERR_NO_SELECT");
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            //var liChuteMgmt = this.ChuteMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();

            //if (liChuteMgmt.Count() <= 0)
            //{
            //    BaseClass.MsgError("ERR_DELETE");
            //}

            //liChuteMgmt.ForEach(p => ChuteMgmtList.Remove(p));


            if (this.ChuteMgmtList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0)
            {
                this.BaseClass.MsgQuestion("ASK_DEL_DB");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
            }

            this.ChuteMgmtList.Where(p => p.IsSelected == true).ToList().ForEach(p =>
            {
                if (p.IsNew != true)
                {
                    p.Checked = false;

                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        this.UpdateSP_CHUTE_UPD(da, p).Wait();
                    }

                }

                this.ChuteMgmtList.Remove(p);
            });
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
            bool bRtnValue = true;

            if (this.ChuteMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #endregion

        #region > 데이터 관련

        #region >> GetSP_CHUTE_LIST_INQ - Chute List 조회
        /// <summary>
        /// 슈트 관리 데이터 조회
        /// </summary>
        private DataSet GetSP_CHUTE_LIST_INQ(string searchedEQPId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "UI_CHUTE_MST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();

            string strEqpId             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);               // 설비 ID
            string strChuteId           = this.txtChuteId.Text.Trim();                                          // 슈트 ID
            string strChuteNm           = this.txtChuteNm.Text.Trim();                                          // 슈트 NAME
            string strChuteTypeCd       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChuteTypeCd);         // 슈트용도코드
            string strUseYn             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);               // 사용 여부
            string strFinalChuteId      = this.txtFinalChuteId.Text.Trim();                                     // 최종 합류 슈트
            #endregion

            #region Input 파라메터
            dicInputParam.Add("EQP_ID",             strEqpId);          // 설비 ID
            dicInputParam.Add("CHUTE_ID",           strChuteId);        // 슈트 ID
            dicInputParam.Add("CHUTE_NM",           strChuteNm);        // 슈트 NAME
            dicInputParam.Add("CHUTE_TYPE_CD",      strChuteTypeCd);    // 슈트용도코드
            dicInputParam.Add("USE_YN",             strUseYn);          // 사용 여부
            dicInputParam.Add("FINAL_CHUTE_ID",     strFinalChuteId);   // 최종 합류 슈트
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> InsertSP_CHUTE_INS - Chute 등록
        /// <summary>
        /// Chute 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_CHUTE_INS(BaseDataAccess _da, ChuteMgmt _item)
        {
            bool isRtnValue = true;
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue = null;
                var strProcedureName = "UI_CHUTE_MST_INS";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
                Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

                string strEqpID = _item.EQP_ID;                            // 설비 ID
                string strChuteId = _item.CHUTE_ID;                        // 슈트 ID
                string strChuteNm = _item.CHUTE_NM;                        // CHUTE_NM
                string strFinalChuteId = _item.FINAL_CHUTE_ID;             // 최종 합류 슈트
                int intChuteAllocPrty = _item.CHUTE_ALLOC_PRTY;         // 슈트 우선순위
                string strPlcChuteId = _item.PLC_CHUTE_ID;                 // PLC_CHUTE_ID
                string strChuteTypeCd = _item.CHUTE_TYPE_CD;               // 슈트 종류 코드
                string strChuteUseCd = _item.CHUTE_USE_CD;                 // 슈트 사용 형태
                string strZONE_ID = _item.ZONE_ID;                         // ZONE_ID
                string strUseYN = _item.Checked == true ? "Y" : "N";       // 사용 여부
                string strUserID = this.BaseClass.UserID;                  // 사용자 ID
                #endregion

                #region Input 파라메터     
                dicInputParam.Add("EQP_ID", strEqpID);                          // 설비 ID
                dicInputParam.Add("CHUTE_ID", strChuteId);                      // 슈트 ID
                dicInputParam.Add("CHUTE_NM", strChuteNm);                      // CHUTE_NM
                dicInputParam.Add("FINAL_CHUTE_ID", strChuteUseCd);             // 최종 합류 슈트
                dicInputParam.Add("CHUTE_ALLOC_PRTY", intChuteAllocPrty);       // 슈트 우선순위
                dicInputParam.Add("PLC_CHUTE_ID", strPlcChuteId);               // PLC_CHUTE_ID
                dicInputParam.Add("CHUTE_TYPE_CD", strChuteTypeCd);             // 슈트 종류 코드
                dicInputParam.Add("CHUTE_USE_CD", strChuteUseCd);               // 슈트 사용 형태
                dicInputParam.Add("P_ZONE_ID", strZONE_ID);                     // ZONE_ID
                dicInputParam.Add("USE_YN", strUseYN);                          // 사용 여부
                dicInputParam.Add("USER_ID", strUserID);                        // 사용자 ID
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
            }
            catch { throw; }

            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_CHUTE_UPD - Chute 수정
        /// <summary>
        /// Chute 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_CHUTE_UPD(BaseDataAccess _da, ChuteMgmt _item)
        {
            bool isRtnValue = true;
            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "UI_CHUTE_MST_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            string strEqpID             = _item.EQP_ID;                             // 설비 ID
            string strChuteId           = _item.CHUTE_ID;                           // 슈트 ID
            string strChuteNm           = _item.CHUTE_NM;                           // CHUTE_NM
            string strFinalChuteId      = _item.FINAL_CHUTE_ID;                     // 최종 합류 슈트
            int    intChuteAllocPrty    = _item.CHUTE_ALLOC_PRTY;                   // 슈트 우선 순위
            string strPlcChuteId        = _item.PLC_CHUTE_ID;                       // PLC_CHUTE_ID
            string strChuteTypeCd       = _item.CHUTE_TYPE_CD;                      // 슈트 종류 코드
            string strChuteUseCd        = _item.CHUTE_USE_CD;                       // 슈트 사용 형태
            string strZONE_ID           = _item.ZONE_ID;                            // ZONE_ID
            string strUseYN             = _item.Checked == true ? "Y" : "N";        // 사용 여부
            string strUserID            = this.BaseClass.UserID;                    // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("EQP_ID", strEqpID);                        // 설비 ID
            dicInputParam.Add("CHUTE_ID", strChuteId);                    // 슈트 ID
            dicInputParam.Add("CHUTE_NM", strChuteNm);                    // CHUTE_NM
            dicInputParam.Add("FINAL_CHUTE_ID", strFinalChuteId);                    // CHUTE_NM
            dicInputParam.Add("CHUTE_ALLOC_PRTY", intChuteAllocPrty);                    // CHUTE_NM
            dicInputParam.Add("PLC_CHUTE_ID", strPlcChuteId);             // PLC_CHUTE_ID
            dicInputParam.Add("CHUTE_TYPE_CD", strChuteTypeCd);                    // CHUTE_NM
            dicInputParam.Add("CHUTE_USE_CD", strChuteUseCd);             // CHUTE_USE_CD
            dicInputParam.Add("ZONE_ID", strZONE_ID);                     // ZONE_ID
            dicInputParam.Add("USE_YN", strUseYN);                        // 사용 여부
            dicInputParam.Add("USER_ID", strUserID);                      // 사용자 ID
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

        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void M1003_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= this.M1003_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void M1003_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Unloaded -= this.M1003_Unloaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region > 슈트 관리
        #region >> 버튼 클릭 이벤트

        #region + 슈트 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangedRowData = this.ChuteMgmtList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    ChuteSearch();
                }
            }
            else
            {
                ChuteSearch();
            }
        }
        #endregion

        #region + 슈트 관리 조회
        /// <summary>
        /// 슈트 관리 조회
        /// </summary>
        private void ChuteSearch()
        {
            SearchedEQPId = null;
            SearchedEQPId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);

            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_CHUTE_LIST_INQ(SearchedEQPId);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                {
                    // 정상 처리된 경우
                    this.ChuteMgmtList = new ObservableCollection<ChuteMgmt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.ChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChuteMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ChuteMgmtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
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

        #region + 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
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
                tv.Add(this.tvMasterGrid);
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

        #region + 슈트 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.ChuteMgmtList.ForEach(p => p.ClearError());

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                foreach (var item in this.ChuteMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.CHUTE_ID) == true)
                        {
                            item.CellError("CHUTE_ID", string.Format(strMessage, this.GetLabelDesc("CHUTE_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.CHUTE_USE_CD) == true)
                        {
                            item.CellError("CHUTE_USE_CD", string.Format(strMessage, this.GetLabelDesc("CHUTE_USE_CD")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.ChuteMgmtList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_CHUTE_INS(da, item).Result;
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_CHUTE_UPD(da, item).Result;
                            }

                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            BaseClass.MsgInfo("CMPT_SAVE");

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                            }

                            // 체크박스 해제
                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSelected = false;
                            }

                            // 저장 후 저장내용 List에 출력 : Header
                            DataSet dsRtnValue = this.GetSP_CHUTE_LIST_INQ(SearchedEQPId);

                            this.ChuteMgmtList = new ObservableCollection<ChuteMgmt>();
                            this.ChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.ChuteMgmtList;
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_MSSQL == BaseEnumClass.TransactionState_MSSQL.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new ChuteMgmt
            {
                EQP_ID = string.Empty,
                CHUTE_ID = string.Empty,
                CHUTE_NM = string.Empty,
                FINAL_CHUTE_ID = string.Empty,
                CHUTE_TYPE_CD = "NML",
                CHUTE_USE_CD = "F",
                ZONE_ID = string.Empty,
                CHUTE_ALLOC_PRTY = 0,
                PLC_CHUTE_ID = string.Empty,
                USE_YN = "Y",
                IsSelected = true,
                IsNew = true,
                IsSaved = false
            };

            this.ChuteMgmtList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle = this.ChuteMgmtList.Count - 1;

            this.ChuteMgmtList[this.ChuteMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.ChuteMgmtList[this.ChuteMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }

            this.DeleteGridRowItem();
        }
        #endregion
        #endregion

        #region >> 그리드 관련 이벤트
        #region + 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.InRowCell)
            {
                //if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

                if (view.ActiveEditor == null)
                {
                    view.ShowEditor();

                    if (view.ActiveEditor == null) { return; }
                    Dispatcher.BeginInvoke(new Action(() => {
                        view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                    }), DispatcherPriority.Render);
                }
            }
        }
        #endregion


        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid") == true)
            {
                ChuteMgmt dataMember = tv.Grid.CurrentItem as ChuteMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "EQP_ID":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "CHUTE_ID":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;

                    default: break;
                }
            }
        }
        #endregion

        #region > 컬럼 데이터 변경 후 이벤트 (컬럼 변경 후 다른 컬럼 값 변경시 사용)
        /// <summary>
        /// 컬럼 데이터 변경 후 이벤트 (컬럼 변경 후 다른 컬럼 값 변경시 사용)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvMasterGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                TableView tv = sender as TableView;
                ChuteMgmt dataMember = tv.Grid.CurrentItem as ChuteMgmt;

                if (e.Column.FieldName.Equals("CHUTE_ID"))
                {
                    if(dataMember.CHUTE_ID.Length > 2) 
                    {
                        dataMember.FINAL_CHUTE_ID = dataMember.CHUTE_ID.Substring(0, 2);
                    }
                }

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            //this.TreeControlRefreshEvent();
        }
        #endregion
        #endregion
    }

}
