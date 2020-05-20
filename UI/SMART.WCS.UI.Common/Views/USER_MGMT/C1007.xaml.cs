using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.UI.COMMON.DataMembers.C1007;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 배포 관리
    /// </summary>
    public partial class C1007 : UserControl, IDisposable
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
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1007()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public C1007(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource      = _liMenuNavigation;
                this.NavigationBar.MenuID           = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 초기화
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
        #region > 배포 서버 파일 리스트
        /// <summary>
        /// 배포 서버 파일 리스트
        /// </summary>
        public static readonly DependencyProperty ServerFileListProperty
            = DependencyProperty.Register("ServerFileList", typeof(ObservableCollection<DeployServerFileList>), typeof(C1007)
                , new PropertyMetadata(new ObservableCollection<DeployServerFileList>()));

        /// <summary>
        /// 배포 서버 파일 리스트
        /// </summary>
        public ObservableCollection<DeployServerFileList> ServerFileList
        {
            get { return (ObservableCollection<DeployServerFileList>)GetValue(ServerFileListProperty); }
            set { SetValue(ServerFileListProperty, value); }
        }
        #endregion

        #region > 로컬 어플리케이션 파일 리스트
        /// <summary>
        /// 로컬 어플리케이션 파일 리스트
        /// </summary>
        public static readonly DependencyProperty LocalFileListProperty
            = DependencyProperty.Register("LocalFileList", typeof(ObservableCollection<DeployLocalFileList>), typeof(C1007)
                , new PropertyMetadata(new ObservableCollection<DeployLocalFileList>()));

        /// <summary>
        /// 로컬 어플리케이션 파일 리스트
        /// </summary>
        public ObservableCollection<DeployLocalFileList> LocalFileList
        {
            get { return (ObservableCollection<DeployLocalFileList>)GetValue(LocalFileListProperty); }
            set { SetValue(LocalFileListProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1007), new PropertyMetadata(string.Empty));

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
            // 콤보박스 설정 - 프로그램 종류
            DataTable dtCommonData                  = this.GetComboBoxCommonData();
            DataTable dtClone                       = null;
            Dictionary<string, string> dicAppName   = new Dictionary<string, string>();

            dicAppName.Add("WCS",       this.BaseClass.GetAppSettings("App_WCS"));
            dicAppName.Add("SCADA",     this.BaseClass.GetAppSettings("App_SCADA"));
            dicAppName.Add("KIOSK",     this.BaseClass.GetAppSettings("App_KIOSK"));

            if (dtCommonData != null)
            {
                if (dtCommonData.Rows.Count > 0)
                {   
                    // 공통코드 테이블 구조를 복제한다.
                    dtClone = dtCommonData.Clone();

                    // 조회한 공통코드
                    foreach (DataRow drRow in dtCommonData.Rows)
                    { 
                        // Config파일에 설정된 프로그램 경로 데이터
                        foreach (KeyValuePair<string, string> item in dicAppName)
                        {
                            if (drRow["CODE"].Equals(item.Key) == true)
                            {
                                DataRow drCloneTableNewRow  = dtClone.NewRow();
                                drCloneTableNewRow["CODE"]  = drRow["CODE"];
                                drCloneTableNewRow["NAME"]  = drRow["NAME"];
                                dtClone.Rows.Add(drCloneTableNewRow);
                            }
                        }
                    }

                    if (dtClone.Rows.Count > 0)
                    {
                        DataRow drNewRow    = dtClone.NewRow();
                        drNewRow["CODE"]    = "99999";
                        drNewRow["NAME"]    = string.Empty;
                        dtClone.Rows.InsertAt(drNewRow, 0);

                        this.cboProgKind.ItemsSource    = SMART.WCS.Common.Data.ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtClone);
                        this.cboProgKind.SelectedIndex  = 0;

                    }
                }
            }
        }
        #endregion

        private void InitEvent()
        {
            this.cboProgKind.SelectedIndexChanged += CboProgKind_SelectedIndexChanged;
        }

        
        #endregion

        #region > GetDeployServerFileInfo_WebService - 웹서비스를 이용하여 배포 서버 파일 리스트를 가져온다.
        /// <summary>
        /// 웹서비스를 이용하여 배포 서버 파일 리스트를 가져온다.
        /// </summary>
        /// <param name="_strSelectedAppName">선택된 어플리케이션 명</param>
        /// <returns></returns>
        private DataTable GetDeployServerFileInfo_WebService(string _strSelectedAppName)
        //private Dictionary<string, DateTime> GetDeployServerFileInfo_WebService(string _strSelectedAppName)
        {
            try
            {
                DataTable dtRtnValue                        = new DataTable();
                Dictionary<string, DateTime> dicRtnValue    = new Dictionary<string, DateTime>();

                //using (WebService_Coupang.CoupangWebServiceClient client = new WebService_Coupang.CoupangWebServiceClient())
                //{
                //    var deployServerFileInfo = client.GetDeployServerFileInfo(_strSelectedAppName);

                //    if (deployServerFileInfo.ResultCD.Equals("0") == false)
                //    {
                //        this.BaseClass.MsgError(deployServerFileInfo.ResultMsg, BaseEnumClass.CodeMessage.MESSAGE);
                //        return null;
                //    }
                    
                //     var arrLiServerFileInfo = deployServerFileInfo.ServerFileInfo;

                //    var aaa = arrLiServerFileInfo[4];
                //    var bbb = aaa.Keys;
                //    var bbb2 = aaa.Keys.GetType();
                //    var ccc = aaa.Values;
                //    var ccc2 = aaa.Values.GetType();

                //    SMART.WCS.Common.Utility.HelperClass.CreateDataTableSchema(dtRtnValue, BaseEnumClass.CreateTableSchemaKind.DEPLOY_SERVER_FILE_LIST);

                //    for (int i = 0; i < arrLiServerFileInfo.Length; i++)
                //    {
                //        DataRow drNewrow = dtRtnValue.NewRow();

                //        foreach (KeyValuePair<string, object> item in arrLiServerFileInfo[i])
                //        {
                //            drNewrow[item.Key] = item.Value;
                            

                //            //var ddd = arrLiServerFileInfo[i];
                //            ////var eee = arrLiServerFileInfo[i].Values;

                //            //for (int j = 0; j < ddd.Count(); j++)
                //            //{
                //            //    int k = 0;

                //            //    var iaaa = ddd[j][0];
                //            //    //foreach (KeyValuePair<string, object> fileItem in ddd)
                //            //    //{
                //            //    //    if (k == 0)
                //            //    //    {
                //            //    //        DataRow drNewRow = dtRtnValue.NewRow();
                //            //    //    }
                //        }

                //        dtRtnValue.Rows.Add(drNewrow);
                //    }

                //    //if (dicRtnValue.Count() > 0)
                //    //{
                //    //    SMART.WCS.Common.Utility.HelperClass.CreateDataTableSchema(dtRtnValue, BaseEnumClass.CreateTableSchemaKind.DEPLOY_SERVER_FILE_LIST);

                //    //    foreach (KeyValuePair<string, DateTime> item in dicRtnValue)
                //    //    {
                //    //        DataRow drNewRow = dtRtnValue.NewRow();
                //    //        drNewRow["SERVER_FILE"]     = item.Key;
                //    //        drNewRow["UPD_DT"]          = item.Value;
                //    //        //drNewRow["APP_DIR"]         = 

                //    //        dtRtnValue.Rows.Add(drNewRow);
                //    //    }
                //    //}
                //}
                
                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetDeployLocalFileInfo - Application별 로컬에 배포된 파일 리스트를 가져온다.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_strSelectedAppName">Application별 로컬에 배포된 파일 리스트를 가져온다.</param>
        /// <returns></returns>
        private DataTable GetDeployLocalFileInfo(string _strSelectedAppName)
        {
            try
            {
                DataTable dtRtnValue                        = new DataTable();
                this.BaseClass.CreateDataTableSchema(dtRtnValue, BaseEnumClass.CreateTableSchemaKind.DEPLOY_LOCAL_FILE_LIST);

                var strAppPath          = this.BaseClass.GetAppSettings($"App_{_strSelectedAppName}");
                var liFileListApp       = this.BaseClass.GetDirectoryFileList(strAppPath);

                foreach (var file in liFileListApp)
                {
                    DataRow drNewRow        = dtRtnValue.NewRow();
                    var fileInfo            = new FileInfo(file);
                    var fileLastTime        = File.GetLastWriteTime($"{strAppPath}\\{fileInfo.Name}");

                    drNewRow["LOCAL_FILE"]  = fileInfo.Name;
                    drNewRow["UPD_DT"]      = fileLastTime;

                    dtRtnValue.Rows.Add(drNewRow);
                }

                List<string> liExistDir     = this.BaseClass.GetListOnDictionary(strAppPath);

                foreach (var item in liExistDir)
                {
                    switch (item)
                    {
                        case "th-TH":
                        case "ko-KR":
                            var strFileLanguagePath     = $"{strAppPath}\\{item}\\";
                            var liFileListLanguage      = this.BaseClass.GetDirectoryFileList(strFileLanguagePath);

                            foreach (var fileLanguage in liFileListLanguage)
                            {
                                DataRow drNewRow        = dtRtnValue.NewRow();
                                var fileInfo            = new FileInfo(fileLanguage);
                                var fileLastTIme        = System.IO.File.GetLastWriteTime($"{strAppPath}\\{item}\\{fileInfo.Name}");

                                drNewRow["LOCAL_FILE"]  = $"{item}\\{fileInfo.Name}";
                                drNewRow["UPD_DT"]      = fileLastTIme;

                                dtRtnValue.Rows.Add(drNewRow);
                            }
                            break;
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
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
            iTotalRowCount = (this.gridLeft.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region GetComboBoxCommonData - 콤보박스 설정 데이터 조회 및 컨트롤 바인딩
        /// <summary>
        /// 콤보박스 설정 데이터 조회 및 컨트롤 바인딩
        /// </summary>
        private DataTable GetComboBoxCommonData()
        {
            var strCommonCD         = "PROG_KIND";
            return SMART.WCS.Common.Control.CommonComboBox.GetCommonData(strCommonCD, null, false);
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 콤보박스 이벤트
        /// <summary>
        /// 콤보박스 선택 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboProgKind_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var strSelectedKey = this.BaseClass.ComboBoxSelectedKeyValue(this.cboProgKind);    // 콤보박스 선택된 코드값

                if (strSelectedKey.Equals("99999") == true)
                {
                    this.gridLeft.ItemsSource       = null;
                    this.gridRight.ItemsSource      = null;
                    
                    return; 
                }

                if (strSelectedKey.Length > 0)
                {
                    // 배포 서버 파일 리스트
                    DataTable dtServerFileList  = this.GetDeployServerFileInfo_WebService(strSelectedKey);
                    // 로컬 파일 리스트
                    DataTable dtLocalFileList   = this.GetDeployLocalFileInfo(strSelectedKey);

                    #region + 서버 파일 리스트 바인딩
                    this.ServerFileList= new ObservableCollection<DeployServerFileList>();
                    this.ServerFileList.ToObservableCollection(dtServerFileList);
                    this.gridLeft.ItemsSource   = this.ServerFileList;
                    #endregion

                    #region 로컬 파일 리스트 바인딩
                    this.LocalFileList= new ObservableCollection<DeployLocalFileList>();
                    this.LocalFileList.ToObservableCollection(dtLocalFileList);
                    this.gridRight.ItemsSource = this.LocalFileList;
                    #endregion

                    this.CompareServerLocalFile();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void CompareServerLocalFile()
        {
            try
            {
                //foreach (var itemLocal in this.LocalFileList)
                //{
                //    foreach (var itemServer in this.ServerFileList)
                //    {
                //        if (itemLocal.LOCAL_FILE.Equals(itemServer.SERVER_FILE) == true)
                //        {
                //            if (itemServer.UPD_DT.Equals(itemLocal.UPD_DT) == false)
                //            {
                //                itemLocal.IsSelected            = true;
                //                itemLocal.BackgroundBrush       = this.BaseClass.ConvertStringToSolidColorBrush("#CDC0B0");
                //                itemLocal.ForegroundBrush       = this.BaseClass.ConvertStringToSolidColorBrush("#3B5998");
                //                itemLocal.APP_DIR               = itemServer.APP_DIR;
                //                itemLocal.DIFF_FLAG             = "D";
                //            }

                //            continue;
                //        }
                //    }
                //}

                var joinNewFileList =   from local in this.LocalFileList
                                        join server in this.ServerFileList on local.LOCAL_FILE equals server.SERVER_FILE into fileGroup
                                        from fileList in fileGroup.DefaultIfEmpty()
                                        select new
                                        {
                                            //    isSelected      = true
                                            //,   fileName        = local.LOCAL_FILE
                                            //,   background      = this.BaseClass.ConvertStringToSolidColorBrush("#CDC0B0")
                                            //,   foreground      = this.BaseClass.ConvertStringToSolidColorBrush("#3B5998")
                                            //,   appDir          = local.APP_DIR
                                            ////,   diffFlag        = "N"
                                            //,   serverFile       = fileList.SERVER_FILE 

                                        };
                
                foreach (var item in joinNewFileList)
                {
                    //if(item.serverFile.Length == 0)
                    //{
                    //    item.
                    //}
                }

            }
            catch { throw; }
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
        // ~C1007()
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
