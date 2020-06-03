using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.M1005
{
    class IfMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public IfMgmt()
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

        #region + IF_YMD - Interface 일자
        private string _IF_YMD;
        public string IF_YMD
        {
            get { return this._IF_YMD; }
            set
            {
                if (this._IF_YMD != value)
                {
                    this._IF_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + START_TIME - 시작 일자
        private string _START_TIME;
        public string START_TIME
        {
            get { return this._START_TIME; }
            set
            {
                if (this._START_TIME != value)
                {
                    this._START_TIME = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + END_TIME - 완료 일자
        private string _END_TIME;
        public string END_TIME
        {
            get { return this._END_TIME; }
            set
            {
                if (this._END_TIME != value)
                {
                    this._END_TIME = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IF_CNT - 전송건수
        private string _IF_CNT;
        public string IF_CNT
        {
            get { return this._IF_CNT; }
            set
            {
                if (this._IF_CNT != value)
                {
                    this._IF_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
