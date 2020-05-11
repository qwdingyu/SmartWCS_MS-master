using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Common.DataMembers.C1098
{
    public class MasterView : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        private bool g_isValidation = false;

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

        #region * 분류그룹명 - CFG_GRP_NM
        private string _CFG_GRP_NM;
        /// <summary>
        /// 분류그룹명
        /// </summary>
        public string CFG_GRP_NM
        {
            get { return this._CFG_GRP_NM; }
            set
            {
                if (this._CFG_GRP_NM != value)
                {
                    this._CFG_GRP_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 분류상세명 - CFG_DTL_NM
        private string _CFG_DTL_NM;
        /// <summary>
        /// 분류상세명
        /// </summary>
        public string CFG_DTL_NM
        {
            get { return this._CFG_DTL_NM; }
            set
            {
                if (this._CFG_DTL_NM != value)
                {
                    this._CFG_DTL_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 소터 최대회전수 - RECIRCULATION_COUNT
        private string _RECIRCULATION_COUNT;
        /// <summary>
        /// 소터 최대회전수
        /// </summary>
        public string RECIRCULATION_COUNT
        {
            get { return this._RECIRCULATION_COUNT; }
            set
            {
                if (this._RECIRCULATION_COUNT != value)
                {
                    this._RECIRCULATION_COUNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 재고슈트 분류방식 - TYPE_ALLOC_NM
        private string _TYPE_ALLOC_NM;
        /// <summary>
        /// 재고슈트 분류방식
        /// </summary>
        public string TYPE_ALLOC_NM
        {
            get { return this._TYPE_ALLOC_NM; }
            set
            {
                if (this._TYPE_ALLOC_NM != value)
                {
                    this._TYPE_ALLOC_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 조회순서 - DISPLAY_SEQ
        private int _DISPLAY_SEQ;
        /// <summary>
        /// 조회순서
        /// </summary>
        public int DISPLAY_SEQ
        {
            get { return this._DISPLAY_SEQ; }
            set
            {
                if (this._DISPLAY_SEQ != value)
                {
                    this._DISPLAY_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 사용유무 - ISVALID
        private string _ISVALID;
        /// <summary>
        /// 사용유무
        /// </summary>
        public string ISVALID
        {
            get { return this._ISVALID; }
            set
            {
                if (this._ISVALID != value)
                {
                    this._ISVALID = value;
                    this.ISVALID_CHECKED= this._ISVALID.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 사용유무 CheckBox
        private bool _ISVALID_CHECKED;
        /// <summary>
        /// 사용유무 CheckBox
        /// </summary>
        public bool ISVALID_CHECKED
        {
            get { return this._ISVALID_CHECKED; }
            set
            {
                if (this._ISVALID_CHECKED != value)
                {
                    this._ISVALID_CHECKED = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 수정일 - DT_UPD
        private string _DT_UPD;
        /// <summary>
        /// 수정일
        /// </summary>
        public string DT_UPD
        {
            get { return this._DT_UPD; }
            set
            {
                if (this._DT_UPD != value)
                {
                    this._DT_UPD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * IS_EDITABLE_RECIRCULATION_COUNT
        private string _IS_EDITABLE_RECIRCULATION_COUNT;
        public string IS_EDITABLE_RECIRCULATION_COUNT
        {
            get { return _IS_EDITABLE_RECIRCULATION_COUNT; }
            set
            {
                if (this._IS_EDITABLE_RECIRCULATION_COUNT != value)
                {
                    this._IS_EDITABLE_RECIRCULATION_COUNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * IS_EDITABLE_TYPE_ALLOC
        private string _IS_EDITABLE_TYPE_ALLOC;
        public string IS_EDITABLE_TYPE_ALLOC
        {
            get { return this._IS_EDITABLE_TYPE_ALLOC; }
            set
            {
                if (this._IS_EDITABLE_TYPE_ALLOC != value)
                {
                    this._IS_EDITABLE_TYPE_ALLOC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 그룹코드 - CFG_GRP_CODE
        private string _CFG_GRP_CODE;
        /// <summary>
        /// 그룹코드
        /// </summary>
        public string CFG_GRP_CODE
        {
            get { return this._CFG_GRP_CODE; }
            set
            {
                if (this._CFG_GRP_CODE != value)
                {
                    this._CFG_GRP_CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * 상세코드 - CFG_DTL_CODE
        private string _CFG_DTL_CODE;
        /// <summary>
        /// 상세코드
        /// </summary>
        public string CFG_DTL_CODE
        {
            get { return this._CFG_DTL_CODE; }
            set
            {
                if (this._CFG_DTL_CODE != value)
                {
                    this._CFG_DTL_CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * TYPE_ALLOC
        private string _TYPE_ALLOC;
        public string TYPE_ALLOC
        {
            get { return this._TYPE_ALLOC; }
            set
            {
                if (this._TYPE_ALLOC != value)
                {
                    this._TYPE_ALLOC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * OB_DTL_CODE
        private string _OB_DTL_CODE;
        public string OB_DTL_CODE
        {
            get { return this._OB_DTL_CODE; }
            set
            {
                if (this._OB_DTL_CODE != value)
                {
                    this._OB_DTL_CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * ID_REG
        private string _ID_REG;
        public string ID_REG
        {
            get { return this._ID_REG; }
            set
            {
                if (this._ID_REG != value)
                {
                    this._ID_REG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * DT_REG
        private string _DT_REG;
        public string DT_REG
        {
            get { return this._ID_REG; }
            set
            {
                if (this._ID_REG != value)
                {
                    this._ID_REG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * ID_UPD
        private string _ID_UPD;
        public string ID_UPD
        {
            get { return this._ID_UPD; }
            set
            {
                if (this._ID_UPD != value)
                {
                    this._ID_UPD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
