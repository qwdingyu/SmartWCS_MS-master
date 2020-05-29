using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.M1002
{
    public class EquipmentMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public EquipmentMgnt()
        {
            this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }
        #endregion

        #region * Validation - 그리드 데이터 유효성 체크
        public void ClearError()
        {
            this.Error = null;
            this.SetErrorInfo(new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo(), null, ErrorType.None);
        }

        public void RowError(string errorText)
        {
            //_IsValidation = true;
            this.Error = errorText;
            //RaisePropertyChanged(ErrorProperty);
        }
        public void CellError(string _ErrorProperty, string _Error)
        {
            g_isValidation = true;
            this.Error = _Error;
            this.ErrorProperty = _ErrorProperty;
            RaisePropertyChanged(_ErrorProperty);
        }

        public void GetPropertyError(string propertyName, DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info)
        {
            if (g_isValidation && ErrorProperty == propertyName)
            {
                SetErrorInfo(info, Error, ErrorType.Critical);
            }
        }

        public void GetError(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info)
        {
            if (Error != null)
            {
                SetErrorInfo(info, Error, ErrorType.Critical);
            }
        }

        void SetErrorInfo(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo Info, string ErrorText, ErrorType errorType)
        {
            Info.ErrorText = ErrorText;
            Info.ErrorType = errorType;
        }

        string _ErrorProperty;
        public string ErrorProperty
        {
            get
            {
                return _ErrorProperty;
            }
            set
            {
                _ErrorProperty = value;
            }
        }
        #endregion

        #region + EQP_ID - 설비 ID
        private string _EQP_ID;
        public string EQP_ID
        {
            get { return this._EQP_ID; }
            set
            {
                if (this._EQP_ID != value)
                {
                    this._EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_NM - 설비 명
        private string _EQP_NM;
        public string EQP_NM
        {
            get { return this._EQP_NM; }
            set
            {
                if (this._EQP_NM != value)
                {
                    this._EQP_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_DESC - 설비 부가 설명
        private string _EQP_DESC;
        public string EQP_DESC
        {
            get { return this._EQP_DESC; }
            set
            {
                if (this._EQP_DESC != value)
                {
                    this._EQP_DESC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_TYPE_CD - 설비 종류 코드
        private string _EQP_TYPE_CD;
        public string EQP_TYPE_CD
        {
            get { return this._EQP_TYPE_CD; }
            set
            {
                if (this._EQP_TYPE_CD != value)
                {
                    this._EQP_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + LINK_EQP_ID - 연결설비ID
        private string _LINK_EQP_ID;
        public string LINK_EQP_ID
        {
            get { return this._LINK_EQP_ID; }
            set
            {
                if (this._LINK_EQP_ID != value)
                {
                    this._LINK_EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + LOC_CD - 위치 코드
        private string _LOC_CD;
        public string LOC_CD
        {
            get { return this._LOC_CD; }
            set
            {
                if (this._LOC_CD != value)
                {
                    this._LOC_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN - 사용 여부
        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    this.Checked = this.USE_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + Checked - 사용여부 변환값
        private bool _Checked;
        public bool Checked
        {
            get { return this._Checked; }
            set
            {
                if (this._Checked != value)
                {
                    this._Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PC_IP - 설비랑 통신하는 PC IP
        private string _PC_IP;
        public string PC_IP
        {
            get { return this._PC_IP; }
            set
            {
                if (this._PC_IP != value)
                {
                    this._PC_IP = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ECS_COMM_NO - 설비 ECS 통신 번호
        private string _ECS_COMM_NO;
        public string ECS_COMM_NO
        {
            get { return this._ECS_COMM_NO; }
            set
            {
                if (this._ECS_COMM_NO != value)
                {
                    this._ECS_COMM_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SER_COMM_NO - 시리얼 통신 번호
        private string _SER_COMM_NO;
        public string SER_COMM_NO
        {
            get { return this._SER_COMM_NO; }
            set
            {
                if (this._SER_COMM_NO != value)
                {
                    this._SER_COMM_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RECIRC_CNT - 순환횟수
        private int _RECIRC_CNT;
        public int RECIRC_CNT
        {
            get { return this._RECIRC_CNT; }
            set
            {
                if (this._RECIRC_CNT != value)
                {
                    this._RECIRC_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ZONE_ID - ZONE ID
        private string _ZONE_ID;
        public string ZONE_ID
        {
            get { return this._ZONE_ID; }
            set
            {
                if (this._ZONE_ID != value)
                {
                    this._ZONE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MAX_READ_CNT - 최대 리딩 횟수
        private int _MAX_READ_CNT;
        public int MAX_READ_CNT
        {
            get { return this._MAX_READ_CNT; }
            set
            {
                if (this._MAX_READ_CNT != value)
                {
                    this._MAX_READ_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MAX_RECIRCULATION - 최대 회전 횟수
        private int _MAX_RECIRCULATION;
        public int MAX_RECIRCULATION
        {
            get { return this._MAX_RECIRCULATION; }
            set
            {
                if (this._MAX_RECIRCULATION != value)
                {
                    this._MAX_RECIRCULATION = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MAX_SCAN_CNT - 최대 바코드 인식 수
        private int _MAX_SCAN_CNT;
        public int MAX_SCAN_CNT
        {
            get { return this._MAX_SCAN_CNT; }
            set
            {
                if (this._MAX_SCAN_CNT != value)
                {
                    this._MAX_SCAN_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IS_RUN_YN - 운영상태
        private string _IS_RUN_YN;
        public string IS_RUN_YN
        {
            get { return this._IS_RUN_YN; }
            set
            {
                if (this._IS_RUN_YN != value)
                {
                    this._IS_RUN_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SORT_PLN_CD - 플랜코드
        private string _SORT_PLN_CD;
        public string SORT_PLN_CD
        {
            get { return this._SORT_PLN_CD; }
            set
            {
                if (this._SORT_PLN_CD != value)
                {
                    this._SORT_PLN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IsSaved - 저장 여부
        private bool _IsSaved;
        public bool IsSaved
        {
            get { return this._IsSaved; }
            set
            {
                if (this._IsSaved != value)
                {
                    this._IsSaved = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
