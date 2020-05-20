using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Common.DataMembers.C1099;
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

namespace SMART.WCS.UI.Common.Views.SYS_MGMT
{
    /// <summary>
    /// C1099.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1099 : UserControl
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
        public C1099()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">메뉴 네비게이션 정보</param>
        public C1099(List<string> _liMenuNavigation)
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
        //ItemSortAnly

        public static readonly DependencyProperty ItemSortAnyListProperty
        = DependencyProperty.Register("ItemSortAnyList", typeof(ObservableCollection<ItemSortAnly>), typeof(C1099)
            , new PropertyMetadata(new ObservableCollection<ItemSortAnly>()));

        public ObservableCollection<ItemSortAnly> ItemSortAnyList
        {
            get { return (ObservableCollection<ItemSortAnly>)GetValue(ItemSortAnyListProperty); }
            set { SetValue(ItemSortAnyListProperty, value); }
        }
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
                this.deFrYmd.DateTime   = DateTime.Now.AddDays(-7);     // 조건 시작일
                this.deToYmd.DateTime   = DateTime.Now;                 // 조건 종료일
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

                // 엑셀 다운로드 버튼 클릭
                this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }

        
        #endregion
        #endregion

        #region > 데이터 관련
        /// <summary>
        /// 소터분류조회
        /// </summary>
        private void GetSP_ITEM_SORT_ANLY_RPT()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "CSP_SP_ITEM_SORT_ANLY_RPT";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();

                var strFrYmd    = this.deFrYmd.DateTime.ToString("yyyyMMddHHmmss");     // 조건 시작일시
                var strToYmd    = this.deToYmd.DateTime.ToString("yyyyMMddHHmmss");     // 조건 종료일시
                var strCaller   = "XXXXX";
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("@IN_FR_YMDHMS",      strFrYmd);          // 조건 시작일시
                dicInputParam.Add("@IN_TO_YMDHMS",      strToYmd);          // 조건 종료일시
                dicInputParam.Add("@IN_CALLER",         strCaller);
                #endregion

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam);
                }
                #endregion

                #region > 사용자 관리 
                if (dsRtnValue == null) { return; }
                
                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                {
                    // 정상 처리된 경우
                    this.ItemSortAnyList = new ObservableCollection<ItemSortAnly>();
                    this.ItemSortAnyList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.ItemSortAnyList.Where(p => p.COL1.ToUpper().Equals("TITLE")).ToList().ForEach(p =>
                    {
                        p.BackgroundBrush           = this.BaseClass.ConvertStringToSolidColorBrush("#E9E9E9");
                        p.FontBoldStyle             = FontWeights.Bold;
                        p.ColHorizontalAlignment    = HorizontalAlignment.Center;
                    });

                    //this.ItemSortAnyList.ForEach(p => p.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#FFFFFF"));
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ItemSortAnyList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                this.gridMaster.ItemsSource = this.ItemSortAnyList;
                #endregion
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.GetSP_ITEM_SORT_ANLY_RPT();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        /// <summary>
        /// 엑셀 다운로드 버튼 클릭
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
