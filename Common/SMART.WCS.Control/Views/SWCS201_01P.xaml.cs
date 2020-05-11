using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
using static SMART.WCS.Common.BaseEnumClass;

namespace SMART.WCS.Control.Views
{
    /// <summary>
    /// 엑셀 업로드 클래스
    /// </summary>
    public partial class SWCS201_01P : Window, IDisposable
    {
        #region ▩ Delegate
        //// 일반 엑셀 업로드 결과를 화면으로 전송
        //public delegate void ExcelUploadDataResult(DataTable _dtResult, string _strFileName, CJFC.Common.EnumClass.ExcelUploadKind _excelUploadKind);
        //public event ExcelUploadDataResult UploadResult;

        //// 오더 정보 업로드 데이터 저장 후 저장 결과 화면으로 전송
        //public delegate void ExcelUploadResultOrd(DataTable _dtResult, string _strGrSprCD, string _strFileName, CJFC.Common.EnumClass.ExcelUploadKind _excelUploadKind);
        //public event ExcelUploadResultOrd UploadResultOrd;
        public delegate void ExcelUploadResult(string _strExcelFileName);
        public event ExcelUploadResult ExcelUploadNo;
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// BaseClass 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 엑셀 업로드 종류 (업무별)
        /// </summary>
        private SMART.WCS.Common.BaseEnumClass.ExcelUploadKind g_enumExcelUploadKind;

        /// <summary>
        /// 엑셀 업로드 데이터 저장 데이터테이블
        /// </summary>
        private DataTable g_dtExcelUploadData;

        /// <summary>
        /// 엑셀 업로드 번호
        /// </summary>
        private string g_strExcelUploadNo;

        /// <summary>
        /// 엑셀 업로드 파일명
        /// </summary>
        private string g_strFileName;
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public SWCS201_01P()
        {
            InitializeComponent();
        }

        public SWCS201_01P(SMART.WCS.Common.BaseEnumClass.ExcelUploadKind _enumExcelUploadKind)
        {
            InitializeComponent();

            // 엑셀 업로드 종류
            this.g_enumExcelUploadKind = _enumExcelUploadKind;

            // 이벤트 초기화
            this.InitEvent();
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > FilePath - 파일 경로
        public static readonly DependencyProperty FilePathProperty =
        DependencyProperty.Register("FilePath", typeof(string), typeof(SWCS201_01P));

        /// <summary>
        /// FilePath
        /// </summary>
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        #endregion

        #region FileFullPath - 파일 전체경로
        public static readonly DependencyProperty FileFullPathProperty =
        DependencyProperty.Register("FileFullPath", typeof(string), typeof(SWCS201_01P));

        /// <summary>
        /// FilePath
        /// </summary>
        public string FileFullPath
        {
            get { return (string)GetValue(FileFullPathProperty); }
            set { SetValue(FileFullPathProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        #region > InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            // 파일 열기 버튼 클릭 이벤트
            this.btnFileOpen.PreviewMouseLeftButtonUp += BtnFileOpen_PreviewMouseLeftButtonUp;
        }
        #endregion

        #region > ModifyDataTableColumnID - 엑셀 데이터 저장 데이터테이블 컬럼 ID 변경
        /// <summary>
        /// 엑셀 데이터 저장 데이터테이블 컬럼 ID 변경
        /// </summary>
        private DataTable ModifyDataTableColumnID()
        {
            switch (this.g_enumExcelUploadKind)
            {
                #region + 오더 생성
                case ExcelUploadKind.OPT_ORD_UPLOAD:
                    this.g_dtExcelUploadData.TableName  = "TB_XML_DATA";

                    this.g_dtExcelUploadData.Columns[0].ColumnName  = "CST_CD";         // [0] 고객사 코드
                    this.g_dtExcelUploadData.Columns[1].ColumnName  = "WAV_NO";         // [1] Wave 번호
                    this.g_dtExcelUploadData.Columns[2].ColumnName  = "ORD_NO";         // [2] 오더 번호
                    this.g_dtExcelUploadData.Columns[3].ColumnName  = "ORD_LINE_NO";    // [3] 오더 라인번호
                    this.g_dtExcelUploadData.Columns[4].ColumnName  = "WRK_PLAN_YMD";   // [4] 출고일자
                    this.g_dtExcelUploadData.Columns[5].ColumnName  = "SHIP_TO_CD";     // [5] 거래처 코드
                    this.g_dtExcelUploadData.Columns[6].ColumnName  = "SKU_CD";         // [6] SKU 코드
                    this.g_dtExcelUploadData.Columns[7].ColumnName  = "SKU_CBM";        // [7] SKU CBM
                    this.g_dtExcelUploadData.Columns[8].ColumnName  = "SKU_WTH_LEN";    // [8] SKU 가로
                    this.g_dtExcelUploadData.Columns[9].ColumnName  = "SKU_VERT_LEN";   // [9] SKU 세로
                    this.g_dtExcelUploadData.Columns[10].ColumnName = "SKU_HGT_LEN";    // [10] SKU 높이
                    this.g_dtExcelUploadData.Columns[11].ColumnName = "SKU_WGT";        // [11] SKU 중량
                    this.g_dtExcelUploadData.Columns[12].ColumnName = "LOC_CD";         // [12] Location 코드
                    this.g_dtExcelUploadData.Columns[13].ColumnName = "PLAN_QTY";       // [13] 계획 수량
                    break;
                #endregion

                #region + 권역관리 
                case ExcelUploadKind.RGN_MGT_UPLOAD:
                    this.g_dtExcelUploadData.TableName  = "TB_XML_DATA";

                    this.g_dtExcelUploadData.Columns[0].ColumnName  = "RGN_CD";     // [0] 권역코드
                    this.g_dtExcelUploadData.Columns[1].ColumnName  = "RGN_NM";     // [1] 권역명
                    break;
                #endregion
            }

            return g_dtExcelUploadData;
        }
        #endregion

        #region > 데이터 관련
        #region >> Save_SP_ORD_UPLOAD - 엑셀 업로드 (오더 생성)
        /// <summary>
        /// 엑셀 업로드 (오더 생성)
        /// </summary>
        /// <param name="_da">데이터베이스 Access 객체</param>
        /// <returns></returns>
        private async Task<bool> Save_SP_ORD_UPLOAD(BaseDataAccess _da)
        {
            try
            {
                bool isRtnValue             = true;
                StringWriter sWriter        = this.BaseClass.ConvertDataTableToStringWriter(this.g_dtExcelUploadData);
                var strUploadNo             = $"ORD_{DateTime.Now.ToString("yyyyMMddHHmmss")}";         // 업로드 번호

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_XLS_UPLD.SP_ORD_UPLD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RTN_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;
                var strUserID           = this.BaseClass.UserID;

                #endregion

                #region Input 파라메터
                //dicInputParam.Add("P_XML_DATA",         sWriter);                   // XML 형식의 엑셀 업로드 데이터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);               // 센터코드
                dicInputParam.Add("P_UPLD_NO",          strUploadNo);               // 업로드 번호
                dicInputParam.Add("P_USER_ID",          strUserID);                 // 사용자 ID
                dicInputParam.Add("P_FILE_NM",          this.g_strFileName);        // 엑셀 업로드 파일명
                #endregion

                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = _da.GetSpDataTableCLOB(strProcedureName, dicInputParam, sWriter, arrOutputParam);
                }).ConfigureAwait(true);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                            this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        // ERR_SAVE - 저장 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
                        isRtnValue = false;
                    }
                }

                // 저장이 된 경우 업로드 번호를 전역 변수에 저장한다.
                this.g_strExcelUploadNo = strUploadNo;

                return isRtnValue;
            }
            catch { return false; }
        }
        #endregion

        private async Task<bool> Save_SP_RGN_LIST_UPLD(BaseDataAccess _da)
        {
            try
            {
                bool isRtnValue             = true;
                StringWriter sWriter        = this.BaseClass.ConvertDataTableToStringWriter(this.g_dtExcelUploadData);
                var strUploadNo             = $"ORD_{DateTime.Now.ToString("yyyyMMddHHmmss")}";         // 업로드 번호

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1018.SP_RGN_LIST_UPLD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;
                var strUserID           = this.BaseClass.UserID;

                #endregion

                #region Input 파라메터
                //dicInputParam.Add("P_XML_DATA",         sWriter);                   // XML 형식의 엑셀 업로드 데이터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);               // 센터코드
                dicInputParam.Add("P_UPLD_NO",          strUploadNo);               // 업로드 번호
                dicInputParam.Add("P_USER_ID",          strUserID);                 // 사용자 ID
                dicInputParam.Add("P_FILE_NM",          this.g_strFileName);        // 엑셀 업로드 파일명
                #endregion

                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = _da.GetSpDataTableCLOB(strProcedureName, dicInputParam, sWriter, arrOutputParam);
                }).ConfigureAwait(true);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                            this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        // ERR_SAVE - 저장 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
                        isRtnValue = false;
                    }
                }

                // 저장이 된 경우 업로드 번호를 전역 변수에 저장한다.
                this.g_strExcelUploadNo = strUploadNo;

                return isRtnValue;
            }
            catch { return false; }
        }

        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        #region >> 파일 선택 버튼 클릭 이벤트
        /// <summary>
        /// 파일 선택 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFileOpen_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.g_dtExcelUploadData == null) { this.g_dtExcelUploadData = new DataTable(); }

            try
            {
                // 엑셀 업로드 데이터를 데이터테이블로 변환
                this.g_dtExcelUploadData = this.BaseClass.ConvertExcelToDataTable(ref this.g_strFileName);

                if (g_strFileName == null) { return; }

                this.FilePath           = (this.g_strFileName.Length > 40) ? this.g_strFileName.Substring(0, 40) + "..." : this.g_strFileName;
                this.FileFullPath       = this.g_strFileName;

                if (this.FileFullPath.Trim().Length == 0) { return; }

                // 데이터테이블 변환 스키마의 컬럼 ID를 설정
                this.g_dtExcelUploadData = this.ModifyDataTableColumnID();
            }
            catch (Exception err)
            {
                this.BaseClass.MsgError(err.ToString());
            }
        }
        #endregion

        #region >> 확인 버튼 클릭 이벤트
        /// <summary>
        /// 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnConfirm_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 파일 선택 여부 확인
            if (this.lblFilePath.Text.Trim().Length == 0)
            {
                // ERR_EMPTY_EXCEL_UPLOAD_FILE - 엑셀 업로드 대상 파일을 선택해주세요.
                this.BaseClass.MsgError("ERR_EMPTY_EXCEL_UPLOAD_FILE");
                return;
            }

            try
            {
                bool isRtnValue         = true;

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        //da.BeginTransaction();

                        switch (this.g_enumExcelUploadKind)
                        {
                            #region 최적화 오더 생성
                            case SMART.WCS.Common.BaseEnumClass.ExcelUploadKind.OPT_ORD_UPLOAD:
                                isRtnValue = await this.Save_SP_ORD_UPLOAD(da);
                                break;
                            #endregion

                            #region 권역관리 (코드, 명) 생성
                            case ExcelUploadKind.RGN_MGT_UPLOAD:
                                isRtnValue = await this.Save_SP_RGN_LIST_UPLD(da);
                                break;
                            #endregion
                        }

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;
                        this.ExcelUploadNo(this.g_strExcelUploadNo);
                        this.Close();
                    }
                    catch
                    {
                        //if (da.TransactionState_Oracle == TransactionState_ORACLE.TransactionStarted)
                        //{
                        //    da.RollbackTransaction();
                        //}

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        throw;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 취소 버튼 이벤트
        private void BtnCancel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //this.ExcelUploadNo(this.g_strExcelUploadNo);

            this.Close();
        }
        #endregion
        #endregion

        #region > 기타 이벤트
        #region >> 창 이동 이벤트
        /// <summary>
        /// 창 이동 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion
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
        // ~SWCS201_01P()
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
