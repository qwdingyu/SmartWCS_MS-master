using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.Analysis.DataMembers.A2001;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.UI.Analysis.Views.SETTING
{
    /// <summary>
    /// 분류설정관리
    /// 추성호
    /// </summary>
    public partial class A2001 : UserControl, TabCloseInterface
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
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public A2001()
        {
            InitializeComponent();
        }

        public A2001(List<string> _liMenuNavigation)
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
        #region > IsEnabled _ Left 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(A2001), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 분류설정관리 Master Data
        public static readonly DependencyProperty MasterViewListProperty
        = DependencyProperty.Register("MasterViewList", typeof(ObservableCollection<MasterView>), typeof(A2001)
            , new PropertyMetadata(new ObservableCollection<MasterView>()));

        public ObservableCollection<MasterView> MasterViewList
        {
            get { return (ObservableCollection<MasterView>)GetValue(MasterViewListProperty); }
            set { SetValue(MasterViewListProperty, value); }
        }
        #endregion

        #region > 분류설정관리 Detail Data
        public static readonly DependencyProperty ChuteMapViewListProperty
        = DependencyProperty.Register("ChuteMapViewList", typeof(ObservableCollection<ChuteMapView>), typeof(A2001)
            , new PropertyMetadata(new ObservableCollection<ChuteMapView>()));

        public ObservableCollection<ChuteMapView> ChuteMapViewList
        {
            get { return (ObservableCollection<ChuteMapView>)GetValue(ChuteMapViewListProperty); }
            set { SetValue(ChuteMapViewListProperty, value); }
        }
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
            try
            {

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
                // 조회버튼 클릭
                this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
                // 저장버튼 클릭
                this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
                //// 추가버튼 클릭
                //this.btnAdd.PreviewMouseLeftButtonUp += BtnAdd_PreviewMouseLeftButtonUp;
                // Row 추가 버튼
                this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
                // Row 삭제 버튼
                this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
                // 그리드 클릭 (마스터)
                this.gridLeft.PreviewMouseLeftButtonUp += GridLeft_PreviewMouseLeftButtonUp;
                // 그리드 클릭 (상세)
                this.gridRight.PreviewMouseLeftButtonUp += GridRight_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> CheckModifyData 
        private bool CheckModifyDataSearch()
        {
            bool bRtnValue = true;
            if (this.MasterViewList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                // 셀 유형관리
                bRtnValue = false;
            }
            else if (this.ChuteMapViewList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (bRtnValue == false)
            {
                // 저장되지 않은 데이터가 있습니다.|계속 조회하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXISTS_NO_SAVE_TO_SEARCH");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckModifyDataClose 
        private bool CheckModifyDataClose()
        {   
            bool bRtnValue = true;
            if (this.MasterViewList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                // 셀 유형관리
                bRtnValue = false;
            }
            else if (this.ChuteMapViewList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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

        #region >> CreateDataTableSchema - 데이터테이블 스키마를 정의한다.
        /// <summary>
        /// 데이터테이블 스키마를 정의한다.
        /// </summary>
        /// <param name="_dtParam">스키마 생성 대상 데이터테이블</param>
        /// <returns></returns>
        private DataTable CreateDataTableSchema(DataTable _dtParam)
        {
            try
            {
                if (_dtParam == null) { _dtParam = new DataTable(); }

                _dtParam.Columns.Add("COL_SEQ",         typeof(int));           // Sequence
                _dtParam.Columns.Add("COL_NM",          typeof(string));        // 컬럼명
                _dtParam.Columns.Add("COL_VALUE",       typeof(string));        // 컬럼값

                return _dtParam;
            }
            catch { throw; }
        }
        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return  this.CheckModifyDataClose();
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_CFG_MST_VIEW - 소터분류설정 마스터
        private void GetSP_CFG_MST_VIEW()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "SP_CFG_MST_VIEW";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();

                var strSprGrpCD         = this.txtSprGrpCD.Text.Trim();     // 분류그룹코드
                var strSprDtlCD         = this.txtSprDtlCD.Text.Trim();     // 분류상세코드
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("@IN_CFG_GRP_CODE",       strSprGrpCD);   // 분류그룹코드
                dicInputParam.Add("@IN_CFG_DTL_CODE",       strSprDtlCD);   // 분류상세코드
                #endregion

                // 상태바 (아이콘)
                this.loadingScreen.IsSplashScreenShown = true;

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam);
                }
                #endregion

                #region > 소터분류설정 마스터
                if (dtRtnValue == null) { return; }
                
                if (dtRtnValue.Columns[0].ColumnName.Equals("CODE"))
                {
                    // 오류가 발생한 경우
                    this.MasterViewList.ToObservableCollection(null);
                    this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                }
                else
                {
                    // 정상 처리된 경우
                    this.MasterViewList = new ObservableCollection<MasterView>();
                    this.MasterViewList.ToObservableCollection(dtRtnValue);
                }

                this.gridLeft.ItemsSource = this.MasterViewList;

                #region + 마스터 조회 데이터가 있는 경우 상세 데이터 조회한다.
                if (this.MasterViewList.Count == 0)
                {
                    this.gridRight.ItemsSource = null;
                }
                else
                {
                    // 그리드 Row 포커서 이동
                    this.BaseClass.SetGridRowAddFocuse(this.gridLeft, 0);

                    MasterView currentItem = tvLeftGrid.Grid.CurrentItem as MasterView;
                    strSprGrpCD         = currentItem.CFG_GRP_CODE;
                    strSprDtlCD         = currentItem.CFG_DTL_CODE;

                    // 소터분류설정 상세
                    this.GetSP_CFG_CHUTE_MAP_VIEW(strSprGrpCD, strSprDtlCD);
                }
                #endregion
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.MsgError(err.Message, BaseEnumClass.CodeMessage.MESSAGE);
                throw; 
            }
            finally
            {
                // 상태바 (아이콘)
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> GetSP_CFG_CHUTE_MAP_VIEW - 소터분류설정 상세
        /// <summary>
        /// 소터분류설정 상세
        /// </summary>
        /// <param name="_strSprGrpCD">분류그룹코드</param>
        /// <param name="_strSprDtlCD">분류상세코드</param>
        private void GetSP_CFG_CHUTE_MAP_VIEW(string _strSprGrpCD, string _strSprDtlCD)
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "SP_CFG_CHUTE_MAP_VIEW";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("@IN_CFG_GRP_CODE",       _strSprGrpCD);      // 분류그룹코드
                dicInputParam.Add("@IN_CFG_DTL_CODE",       _strSprDtlCD);      // 분류상세코드
                #endregion

                //// 상태바 (아이콘)
                //this.loadingScreen.IsSplashScreenShown = true;

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam);
                }
                #endregion

                #region > 소터분류설정 상세
                if (dtRtnValue == null) { return; }

                if (dtRtnValue.Columns[0].ColumnName.Equals("CODE"))
                {
                    // 오류가 발생한 경우
                    this.ChuteMapViewList.ToObservableCollection(null);
                    this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                }
                else
                {
                    // 정상 처리된 경우
                    this.ChuteMapViewList = new ObservableCollection<ChuteMapView>();
                    this.ChuteMapViewList.ToObservableCollection(dtRtnValue);
                }

                this.gridRight.ItemsSource = this.ChuteMapViewList;
                #endregion
            }
            catch { throw; }
            finally
            {
                //// 상태바 (아이콘)
                //this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        private bool SaveSP_CFG_MST_SAVE(BaseDataAccess _da, DataTable _dtParamMaster)
        {
            try
            {
                bool isRtnValue = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue = null;
                var strProcedureName = "SP_CFG_MST_SAVE";
                Dictionary<string, object> dicInputParam                                = new Dictionary<string, object>();
                Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutputParam    = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
                Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();
                string strOutputData = string.Empty;

                var strExecJob = "MST";
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("@IN_EXEC_JOB", strExecJob);        // 실행 업무
                dicInputParam.Add("@IN_TB_INFO", _dtParamMaster);
                #endregion

                #region + Output 파라메터
                dicOutputParam.Add("@OUT_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
                #endregion

                //// 상태바 (아이콘)  
                //this.loadingScreen.IsSplashScreenShown = true;

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    //dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam);
                    dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutputParam, ref dicRtnValue);

                    foreach (var item in dicRtnValue)
                    {
                        if (item.Key.ToString() == "@OUT_MSG")
                        {
                            strOutputData = item.Value.ToString();
                        }
                    }
                }
                #endregion

                return isRtnValue;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                return false;
            }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        /// <summary>
        /// 조회버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 조회전 그리드에 추가,수정된 데이터가 있는지 체크
                if (this.CheckModifyDataSearch() == false) { return; }

                this.GetSP_CFG_MST_VIEW();

                //DataTable dtParam = new DataTable();
                //dtParam.Columns.Add("CODE",         typeof(string));
                //dtParam.Columns.Add("NAME",         typeof(string));
                //dtParam.Columns.Add("ETC",          typeof(string));


                //DataRow drNew = null;

                //for (int i= 0; i < 5; i++)
                //{
                //    drNew = dtParam.NewRow();
                //    drNew["CODE"]       = $"CODE_{i.ToString()}";
                //    drNew["NAME"]       = $"NAME_{i.ToString()}";
                //    drNew["ETC"]        = $"ETC_{i.ToString()}";
                //    dtParam.Rows.Add(drNew);
                //}

                //#region + 파라메터 변수 선언 및 값 할당
                //DataTable dtRtnValue                        = null;
                //var strProcedureName                        = "WSP_SP_TEST";
                //Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                //#endregion

                //#region + Input 파라메터
                //dicInputParam.Add("P_INPUT1",       dtParam);
                //#endregion

                //#region + 데이터 조회
                //using (BaseDataAccess dataAccess = new BaseDataAccess())
                //{
                //    dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam);
                //}
                //#endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 저장버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataTable dtParamMaster = new DataTable();
                DataTable dtParamChute  = new DataTable();

                dtParamMaster = this.CreateDataTableSchema(dtParamMaster);

                var liSelectedMaster            = this.MasterViewList.Where(p => p.IsSelected && p.IsUpdate).ToList();
                var liSelectedChuteView         = this.ChuteMapViewList.Where(p => p.IsSelected && p.IsUpdate).ToList();

                if (liSelectedMaster.Count == 0 && liSelectedChuteView.Count == 0)
                {
                    // 저장할 데이터가 없습니다.
                    this.BaseClass.MsgError("");
                    return;
                }

                #region + 소터분류설정 마스터 저장 대상 데이터
                var dtMaster        = ConvertListToDataTable.ConvertToDataTable(liSelectedMaster);
                dtMaster.Columns.RemoveAt(0);

                for (int i = dtMaster.Columns.Count - 1; i >= 16; i--)
                {
                    dtMaster.Columns.RemoveAt(i);
                }

                DataRow drRow       = null;
                int iSeqNo          = 0;

                for (int i = 0; i < dtMaster.Rows.Count; i++)
                {
                    iSeqNo++;

                    for (int j = 0; j < dtMaster.Columns.Count; j++)
                    {
                        drRow       = dtParamMaster.NewRow();
                        drRow["COL_SEQ"]        = iSeqNo;                               // 순번
                        drRow["COL_NM"]         = dtMaster.Columns[j].ColumnName;       // 컬럼명
                        drRow["COL_VALUE"]      = dtMaster.Rows[i][j].ToString();       // 컬럼값
                        dtParamMaster.Rows.Add(drRow);
                    }
                }
                #endregion

                #region + 소터분류설정 상세 저장 대상 데이터 
                var dtChute     = ConvertListToDataTable.ConvertToDataTable(liSelectedChuteView);
                dtChute.Columns.RemoveAt(0);

                for (int i = dtChute.Columns.Count - 1; i >= 16; i--)
                {
                    dtChute.Columns.RemoveAt(i);
                }

                drRow       = null;
                iSeqNo      = 0;

                for (int i = 0; i < dtChute.Rows.Count; i++)
                {
                    iSeqNo++;

                    for (int j = 0; j < dtChute.Columns.Count; j++)
                    {
                        drRow                   = dtParamChute.NewRow();
                        drRow["COL_SEQ"]        = iSeqNo;                               // 순번
                        drRow["COL_NM"]         = dtChute.Columns[j].ColumnName;        // 컬럼명
                        drRow["COL_VALUE"]      = dtChute.Rows[i][j].ToString();        // 컬럼값
                        dtParamChute.Rows.Add(drRow);
                    }
                }
                #endregion

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var isRtnValue = this.SaveSP_CFG_MST_SAVE(da, dtParamMaster);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        

        /// <summary>
        /// 추가버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// Row 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int iCurrentGridRow = this.BaseClass.GetCurrentGridControlRowIndex(this.tvLeftGrid);
                var strCfgGrpCD     = this.MasterViewList[iCurrentGridRow].CFG_GRP_CODE;    // 그룹코드
                var strCfgGrpNM     = this.MasterViewList[iCurrentGridRow].CFG_GRP_NM;      // 그룹명
                var strCfgDtlCD     = this.MasterViewList[iCurrentGridRow].CFG_DTL_CODE;    // 상세코드
                var strCfgDtlNM     = this.MasterViewList[iCurrentGridRow].CFG_DTL_NAME;    // 상세명
                var strTypeAllocNM  = this.MasterViewList[iCurrentGridRow].TYPE_ALLOC_NM;
                var strTypeAlloc    = this.MasterViewList[iCurrentGridRow].TYPE_ALLOC;

                var newItme = new ChuteMapView
                {
                        CFG_GRP_NM              = strCfgGrpNM                                   // 그룹명
                    ,   CFG_DTL_NM              = strCfgDtlNM                                   // 상세명
                    ,   CHUTE_NO                = "0"                                           // 슈트번호
                    ,   FR_RCV_DAYS             = "0"                                           // 재고소요일수 (시작)
                    ,   TO_RCV_DAYS             = "0"                                           // 재고소요일수 (종료)
                    ,   ISVALID                 = "Y"                                           // 사용유무
                    ,   ISVALID_CHECKED         = true
                    ,   DT_UPD                  = DateTime.Now                                  // 수정일시

                    ,   IS_EDITABLE_RCV_DAYS    = "N"
                    ,   DISPLAY_SEQ             = 0
                    ,   CFG_GRP_CODE            = strCfgGrpCD                                   // 그룹코드
                    ,   CFG_DTL_CODE            = strCfgDtlCD                                   // 상세코드
                    ,   STATUS_SORTER           = string.Empty
                    ,   TYPE_ALLOC_NM           = strTypeAllocNM
                    ,   TYPE_ALLOC              = strTypeAlloc
                    ,   RECIRCULATION_COUNT     = "0"
                    ,   ID_REG                  = string.Empty
                    ,   DT_REG                  = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    ,   ID_UPD                  = string.Empty

                    ,   IsSelected              = true
                    ,   IsNew                   = true
                };

                this.ChuteMapViewList.Add(newItme);
                this.gridRight.Focus();
                this.gridRight.CurrentColumn            = this.gridRight.Columns.First();
                this.gridRight.View.FocusedRowHandle    = this.ChuteMapViewList.Count - 1;

                this.ChuteMapViewList[this.ChuteMapViewList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
                this.ChuteMapViewList[this.ChuteMapViewList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

                this.BaseClass.SetGridRowAddFocuse(this.gridRight, this.ChuteMapViewList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// Row 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 체크박스 선택이 없는 경우 구문을 리턴한다.
                if (this.ChuteMapViewList.Where(p => p.IsSelected).ToList().Count() == 0) { return; }

                // 행 추가된 그리드의 Row 중 선택된 Row를 삭제한다.
                this.ChuteMapViewList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
                {
                    this.ChuteMapViewList.Remove(p);
                });

                this.BaseClass.SetGridRowAddFocuse(this.gridRight, this.ChuteMapViewList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 그리드 이벤트
        /// <summary>
        /// 소터분류설정 마스터 그리드 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell)
                {
                    if (view.ActiveEditor == null)
                    {
                        switch (hi.Column.FieldName)
                        {
                            case "RECIRCULATION_COUNT":
                            case "TYPE_ALLOC_NM":
                            case "DISPLAY_SEQ":
                            case "ISVALID":
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                Dispatcher.BeginInvoke(new Action(() => {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                                break;
                        }
                    }

                    int iCurrentClickIndex  = hi.RowHandle;
                    var objGrpCD        = this.gridLeft.GetCellValue(iCurrentClickIndex, this.gridLeft.Columns["CFG_GRP_CODE"]);    // 그룹코드
                    var objDtlCD        = this.gridLeft.GetCellValue(iCurrentClickIndex, this.gridLeft.Columns["CFG_DTL_CODE"]);    // 상세코드

                    this.GetSP_CFG_CHUTE_MAP_VIEW(objGrpCD.ToString(), objDtlCD.ToString());
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 소터분류설정 상세 그리드 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell)
                {
                    if (view.ActiveEditor == null)
                    {
                        switch (hi.Column.FieldName)
                        {
                            case "CHUTE_NO":
                            case "FR_RCV_DAYS":
                            case "TO_RCV_DAYS":
                            case "ISVALID":
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                Dispatcher.BeginInvoke(new Action(() => {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                                break;
                        }
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

                #region + Master Grid
                TableView tv = sender as TableView;
                MasterView masterView = tv.Grid.CurrentItem as MasterView;
                if (masterView == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "RECIRCULATION_COUNT":
                        if (masterView.IS_EDITABLE_RECIRCULATION_COUNT.Equals("N"))
                        {
                            e.Cancel = true;
                        }
                        break;

                    case "TYPE_ALLOC_NM":
                        if (masterView.IS_EDITABLE_TYPE_ALLOC.Equals("N"))
                        {
                            e.Cancel = true;
                        }
                        break;
                }
                #endregion

                #region + Detail Grid
                ChuteMapView mapView = tv.Grid.CurrentItem as ChuteMapView;
                if (mapView == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "FR_RCV_DAYS":
                    case "TO_RCV_DAYS":
                        if (mapView.IS_EDITABLE_RCV_DAYS.Equals("N"))
                        {
                            e.Cancel = true;
                        }
                        break;
                }
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            try
            {
                this.TreeControlRefreshEvent();
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
