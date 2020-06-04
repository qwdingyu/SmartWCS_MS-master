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
using SMART.WCS.UI.COMMON.DataMembers.M1004;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// M1004.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class M1004 : UserControl
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
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// Base Info 선언
        /// </summary>
        private BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public M1004()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public M1004(List<string> _liMenuNavigation)
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
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }
        #endregion

        #region > 그리드 - I/F 리스트
        public static readonly DependencyProperty OrgEsMgmtListProperty
            = DependencyProperty.Register("OrgEsMgmtList", typeof(ObservableCollection<OrgEsMgmt>), typeof(M1004)
                , new PropertyMetadata(new ObservableCollection<OrgEsMgmt>()));

        private ObservableCollection<OrgEsMgmt> OrgEsMgmtList
        {
            get { return (ObservableCollection<OrgEsMgmt>)GetValue(OrgEsMgmtListProperty); }
            set { SetValue(OrgEsMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(M1004), new PropertyMetadata(string.Empty));

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
        #endregion

        #region > 데이터 관련

        #region >> UI_ORG_MST_INQ - 조직 마스터 조회
        /// <summary>
        /// 조직 관리 데이터 조회
        /// </summary>
        private DataSet Get_UI_ORG_MST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "UI_ORG_MST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();

            string strDeCntrNm = this.txtCntrNm.Text.Trim();
            string strDeTmlNm = this.txtTmlNm.Text.Trim();
            string strDeDlvNm = this.txtDlvNm.Text.Trim();
            #endregion

            #region Input 파라메터
            dicInputParam.Add("DE_CNTR_NM", strDeCntrNm);              
            dicInputParam.Add("DE_TML_NM", strDeTmlNm);              
            dicInputParam.Add("DE_DLV_NM", strDeDlvNm);        
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
        #endregion

        #endregion

        #region ▩ 이벤트

        #region > Loaded/Unload 이벤트
        private void M1004_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= this.M1004_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void M1004_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Unloaded -= this.M1004_Unloaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 조직 마스터 관리
        #region >> 버튼 클릭 이벤트

        #region + 조직 마스터 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 조직 마스터 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;
                OreEsSearch();
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

        #region + 조직 마스터 관리 조회
        /// <summary>
        /// 조직 마스터 관리 조회
        /// </summary>
        private void OreEsSearch()
        {
            // 셀 유형관리 데이터 조회
            DataSet dsRtnValue = this.Get_UI_ORG_MST_INQ();

            if (dsRtnValue == null) { return; }

            var strErrCode = string.Empty;
            var strErrMsg = string.Empty;

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
            {
                // 정상 처리된 경우
                this.OrgEsMgmtList = new ObservableCollection<OrgEsMgmt>();
                // 오라클인 경우 TableName = TB_COM_MENU_MST
                this.OrgEsMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
            }
            else
            {
                // 오류가 발생한 경우
                this.OrgEsMgmtList.ToObservableCollection(null);
                BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
            }

            // 조회 데이터를 그리드에 바인딩한다.
            this.gridMaster.ItemsSource = this.OrgEsMgmtList;

            // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
            this.SetResultText();
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
