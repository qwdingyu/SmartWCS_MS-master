using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1001
{
    public class ItemSortAnly : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        #region * 그리드 색상 설정
        public ItemSortAnly()
        {
            //this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
            this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#FFFFFF");
        }

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }
        #endregion

        #region * 컬럼 Bold 처리
        public FontWeight FontBoldStyle { get; set; }
        #endregion

        #region * 컬럼 정렬
        public HorizontalAlignment ColHorizontalAlignment { get; set; }
        #endregion

        #region * COL1
        private string _COL1;
        public string COL1
        {
            get { return this._COL1; }
            set
            {
                if (this._COL1 != value)
                {
                    this._COL1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL2
        private string _COL2;
        public string COL2
        {
            get { return this._COL2; }
            set
            {
                if (this._COL2 != value)
                {
                    this._COL2 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL3
        private string _COL3;
        public string COL3
        {
            get { return this._COL3; }
            set
            {
                if (this._COL3 != value)
                {
                    this._COL3 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL4
        private string _COL4;
        public string COL4
        {
            get { return this._COL4; }
            set
            {
                if (this._COL4 != value)
                {
                    this._COL4 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL5
        private string _COL5;
        public string COL5
        {
            get { return this._COL5; }
            set
            {
                if (this._COL5 != value)
                {
                    this._COL5 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL6
        private string _COL6;
        public string COL6
        {
            get { return this._COL6; }
            set
            {
                if (this._COL6 != value)
                {
                    this._COL6 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL7
        private string _COL7;
        public string COL7
        {
            get { return this._COL7; }
            set
            {
                if (this._COL7 != value)
                {
                    this._COL7 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL8
        private string _COL8;
        public string COL8
        {
            get { return this._COL8; }
            set
            {
                if (this._COL8 != value)
                {
                    this._COL8 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL9
        private string _COL9;
        public string COL9
        {
            get { return this._COL9; }
            set
            {
                if (this._COL9 != value)
                {
                    this._COL9 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL10
        private string _COL10;
        public string COL10
        {
            get { return this._COL10; }
            set
            {
                if (this._COL10 != value)
                {
                    this._COL10 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL11
        private string _COL11;
        public string COL11
        {
            get { return this._COL11; }
            set
            {
                if (this._COL11 != value)
                {
                    this._COL11 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL12
        private string _COL12;
        public string COL12
        {
            get { return this._COL12; }
            set
            {
                if (this._COL12 != value)
                {
                    this._COL12 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL13
        private string _COL13;
        public string COL13
        {
            get { return this._COL13; }
            set
            {
                if (this._COL13 != value)
                {
                    this._COL13 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL14
        private string _COL14;
        public string COL14
        {
            get { return this._COL14; }
            set
            {
                if (this._COL14 != value)
                {
                    this._COL14 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL15
        private string _COL15;
        public string COL15
        {
            get { return this._COL15; }
            set
            {
                if (this._COL15 != value)
                {
                    this._COL15 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * COL9000
        private string _COL9000;
        public string COL9000
        {
            get { return this._COL9000; }
            set
            {
                if (this._COL9000 != value)
                {
                    this._COL9000 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * ID_GRP
        private string _ID_GRP;
        public string ID_GRP
        {
            get { return this._ID_GRP; }
            set
            {
                if (this._ID_GRP != value)
                {
                    this._ID_GRP = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region * SEQ_DISPLAY
        private int _SEQ_DISPLAY;
        public int SEQ_DISPLAY
        {
            get { return this._SEQ_DISPLAY; }
            set
            {
                if (this._SEQ_DISPLAY != value)
                {
                    this._SEQ_DISPLAY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
