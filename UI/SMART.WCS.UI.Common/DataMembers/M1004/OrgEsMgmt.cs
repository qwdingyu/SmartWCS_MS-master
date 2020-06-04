using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.M1004
{
    class OrgEsMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public OrgEsMgmt()
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

        #region + DE_CNTR_CD - 센터코드
        private string _DE_CNTR_CD;
        public string DE_CNTR_CD
        {
            get { return this._DE_CNTR_CD; }
            set
            {
                if (this._DE_CNTR_CD != value)
                {
                    this._DE_CNTR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DE_CNTR_NM - 센터명
        private string _DE_CNTR_NM;
        public string DE_CNTR_NM
        {
            get { return this._DE_CNTR_NM; }
            set
            {
                if (this._DE_CNTR_NM != value)
                {
                    this._DE_CNTR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DE_TML_CD - 터미널 코드
        private string _DE_TML_CD;
        public string DE_TML_CD
        {
            get { return this._DE_TML_CD; }
            set
            {
                if (this._DE_TML_CD != value)
                {
                    this._DE_TML_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DE_TML_NM - 터미널 명
        private string _DE_TML_NM;
        public string DE_TML_NM
        {
            get { return this._DE_TML_NM; }
            set
            {
                if (this._DE_TML_NM != value)
                {
                    this._DE_TML_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DE_DLV_CD - 집배점 코드
        private string _DE_DLV_CD;
        public string DE_DLV_CD
        {
            get { return this._DE_DLV_CD; }
            set
            {
                if (this._DE_DLV_CD != value)
                {
                    this._DE_DLV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DE_DLV_NM - 집배점 명
        private string _DE_DLV_NM;
        public string DE_DLV_NM
        {
            get { return this._DE_DLV_NM; }
            set
            {
                if (this._DE_DLV_NM != value)
                {
                    this._DE_DLV_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ES_CD - ES 코드
        private string _ES_CD;
        public string ES_CD
        {
            get { return this._ES_CD; }
            set
            {
                if (this._ES_CD != value)
                {
                    this._ES_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ES_NM - ES 명
        private string _ES_NM;
        public string ES_NM
        {
            get { return this._ES_NM; }
            set
            {
                if (this._ES_NM != value)
                {
                    this._ES_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + APPLY_YMD - 전송건수
        private string _APPLY_YMD;
        public string APPLY_YMD
        {
            get { return this._APPLY_YMD; }
            set
            {
                if (this._APPLY_YMD != value)
                {
                    this._APPLY_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
