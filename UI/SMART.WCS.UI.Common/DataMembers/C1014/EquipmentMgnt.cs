using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1014
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

        #region + EQP_TYPE_CD - 설비 종류
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

        #region + IS_RUN_YN - 운영 상태
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

        #region +   SORT_PLN_CD - 플랜 코드
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

        #region +   ATTR01 - 속성 1
        private string _ATTR01;
        public string ATTR01
        {
            get { return this._ATTR01; }
            set
            {
                if (this._ATTR01 != value)
                {
                    this._ATTR01 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR02 - 속성 02
        private string _ATTR02;
        public string ATTR02
        {
            get { return this._ATTR02; }
            set
            {
                if (this._ATTR02 != value)
                {
                    this._ATTR02 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR03 - 속성 03
        private string _ATTR03;
        public string ATTR03
        {
            get { return this._ATTR03; }
            set
            {
                if (this._ATTR03 != value)
                {
                    this._ATTR03 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR04 - 속성 04
        private string _ATTR04;
        public string ATTR04
        {
            get { return this._ATTR04; }
            set
            {
                if (this._ATTR04 != value)
                {
                    this._ATTR04 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR05 - 속성 05
        private string _ATTR05;
        public string ATTR05
        {
            get { return this._ATTR05; }
            set
            {
                if (this._ATTR05 != value)
                {
                    this._ATTR05 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR06 - 속성 06
        private string _ATTR06;
        public string ATTR06
        {
            get { return this._ATTR06; }
            set
            {
                if (this._ATTR06 != value)
                {
                    this._ATTR06 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR07 - 속성 07
        private string _ATTR07;
        public string ATTR07
        {
            get { return this._ATTR07; }
            set
            {
                if (this._ATTR07 != value)
                {
                    this._ATTR07 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR08 - 속성 08
        private string _ATTR08;
        public string ATTR08
        {
            get { return this._ATTR08; }
            set
            {
                if (this._ATTR08 != value)
                {
                    this._ATTR08 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR09 - 속성 09
        private string _ATTR09;
        public string ATTR09
        {
            get { return this._ATTR09; }
            set
            {
                if (this._ATTR09 != value)
                {
                    this._ATTR09 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region +   ATTR10 - 속성 10
        private string _ATTR10;
        public string ATTR10
        {
            get { return this._ATTR10; }
            set
            {
                if (this._ATTR10 != value)
                {
                    this._ATTR10 = value;
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
