using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Main
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : BaseApp
    {
        private readonly BaseClass BaseClass = new BaseClass();
        private readonly BaseInfo BaseInfo = new BaseInfo();

        public App()
        {
            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {

                this.BaseClass.CompanyCode = this.BaseClass.GetAppSettings("CompanyCode");

                // 데이터베이스 종류를 설정한다.
                this.BaseClass.MainDatabase = this.BaseClass.GetAppSettings("MainDatabase");

                // App.config에 설정한 Database 연결 타입을 가져온다.
                var strDBConnectionType = this.BaseClass.GetAppSettings("DBConnectType_DEV_REAL");

                string[] args = Environment.GetCommandLineArgs();

                DataSet dsRtnValue = this.GetDatabaseConnectionInfo(strDBConnectionType);
                if (dsRtnValue == null) { return; }
                 
                using (Login frmLogin = new Login(dsRtnValue.Tables[0]))
                {
                    //LoginSample frmLogin = new LoginSample(dtRtnValue);
                    frmLogin.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("프로그램 실행 중 오류가 발생했습니다.\r\n" + "(" + err.Message + ")" + "\r\n프로그램이 종료됩니다.");
                this.BaseClass.Error(err);
                App.Current.Shutdown();
            }
        }

        #region GetDatabaseConnectionInfo - 프로그램 실행 후 센터 정보와, 데이터베이스 연결 문자열을 조회한다.
        /// <summary>
        /// 프로그램 실행 후 센터별 데이터베이스 연결 문자열을 조회한다.
        /// </summary>
        /// <param name="_strDBConnectionType">데이터베이션 연결 타입</param>
        /// <returns></returns>
        private DataSet GetDatabaseConnectionInfo(string _strDBConnectionType)
        {
            try
            {
                DataSet dsRtnValue                        = null;
                var strProcedureName                        = "CSP_C1001_SP_CNTR_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                var strErrCode                              = string.Empty;
                var strErrMsg                               = string.Empty;

                using (FirstDataAccess da = new FirstDataAccess())
                {
                    dsRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam);

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                    {
                        return dsRtnValue;
                    }
                    else
                    {
                        dsRtnValue = null;
                        MessageBox.Show("프로그램 실행 중 오류가 발생했습니다.\r\n" + "(" + strErrMsg + ")" + "\r\n프로그램이 종료됩니다.");
                        App.Current.Shutdown();
                    }
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion

        private static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return false;
            }
            catch { throw; }
        }
    }
}
