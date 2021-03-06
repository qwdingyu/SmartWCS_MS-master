﻿using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.COMMON.DataMembers.M1002;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// M1002.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class M1002 : UserControl, TabCloseInterface
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
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        #endregion

        #region ▩ 생성자
        public M1002()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public M1002(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                //this.InitComboBoxInfo();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }

            //catch
            //{
            //    throw;
            //}
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(M1002), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void IsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TableView view = source as TableView;
                view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region > 그리드 - 설비 리스트
        public static readonly DependencyProperty EquipmentMgntListProperty
            = DependencyProperty.Register("EquipmentMgntList", typeof(ObservableCollection<EquipmentMgnt>), typeof(M1002)
                , new PropertyMetadata(new ObservableCollection<EquipmentMgnt>()));

        public ObservableCollection<EquipmentMgnt> EquipmentMgntList
        {
            get { return (ObservableCollection<EquipmentMgnt>)GetValue(EquipmentMgntListProperty); }
            set { SetValue(EquipmentMgntListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(M1002), new PropertyMetadata(string.Empty));

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
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_LOCCode = { "KY", string.Empty, string.Empty, string.Empty };

            // 콤보박스 - 조회 (사용여부, 설비 종류 코드, 위치 코드)
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, false);
            this.BaseClass.BindingCommonComboBox(this.cboEqpTypeCd, "EQP_TYPE_CD", null, true);

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");
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
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave_First.PreviewMouseLeftButtonUp += BtnSave_First_PreviewMouseLeftButtonUp;

            // 행 추가
            this.btnRowAdd_First.PreviewMouseLeftButtonUp += BtnRowAdd_First_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete_First.PreviewMouseLeftButtonUp += BtnRowDelete_First_PreviewMouseLeftButtonUp;
            #endregion
            
            #region + 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            // Equipment 리스트 그리드 순번 채번을 위한 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;

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

        #region >> CheckGridRowSelected - 그리드 체크박스 선택 유효성 체크
        /// <summary>
        /// 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue = true;
                int iCheckedCount = 0;

                iCheckedCount = this.EquipmentMgntList.Where(p => p.IsSelected == true).Count();
                 
                if (iCheckedCount == 0)
                {
                    bRtnValue = false;
                    BaseClass.MsgError("ERR_NO_SELECT");
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다.
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            //var liEquipmentMgnt = this.EquipmentMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();

            //if (liEquipmentMgnt.Count() <= 0)
            //{
            //    BaseClass.MsgError("ERR_DELETE");
            //}

            //liEquipmentMgnt.ForEach(p => EquipmentMgntList.Remove(p));

            if (this.EquipmentMgntList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0)
            {
                this.BaseClass.MsgQuestion("ASK_DEL_DB");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
            }

            this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList().ForEach(p =>
            {
                if (p.IsNew != true)
                {
                    p.Checked = false;

                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        this.UpdateSP_EQP_UPD(da, p).Wait();
                    }

                }

                this.EquipmentMgntList.Remove(p);
            });
        }

        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return this.CheckModifyData();
        }
        #endregion

        #region >> CheckModifyData - 각 탭의 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue = true;

            if (this.EquipmentMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #endregion

        #region > 데이터 관련

        #region >> GetSP_EQP_LIST_INQ - Equipment List 조회
        /// <summary>
        /// 설비 관리 데이터조회
        /// </summary>
        private DataSet GetSP_EQP_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "UI_EQIP_MST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            
            var strEqpID = this.txtEqpId_First.Text.Trim();                                         // 설비 ID
            var strEqpNm = this.txtEqpNm_First.Text.Trim();                                         // 설비 명
            var strEqpTypeCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpTypeCd);          // 설비 종류 코드
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);            // 사용 여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("EQP_ID", strEqpID);                    // 설비 ID
            dicInputParam.Add("EQP_NM", strEqpNm);                    // 설비 명
            dicInputParam.Add("EQP_TYPE_CD", strEqpTypeCd);           // 설비 종류 코드
            dicInputParam.Add("USE_YN", strUseYn);                    // 사용 여부
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

        #region >> InsertSP_EQP_INS - Equipment 등록
        /// <summary>
        /// Equipment 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_EQP_INS(BaseDataAccess _da, EquipmentMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "UI_EQIP_MST_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strEqpID            = _item.EQP_ID;                         // 설비 ID
            var strEqpNm            = _item.EQP_NM;                         // 설비 명
            var strEqpDesc          = _item.EQP_DESC;                       // 설비 세부 정보
            var strEqpTypeCd        = _item.EQP_TYPE_CD;                    // 설비 종류 코드
            var strLinkEqpId        = _item.LINK_EQP_ID;                    // 연결설비ID
            var strLocCd            = _item.LOC_CD;                         // 위치 코드
            var strPcIp             = _item.PC_IP;                          // 설비와 통신하는 PC IP
            var strEcsCommNo        = _item.ECS_COMM_NO;                    // ECS 통신번호
            var strSerCommNo        = _item.SER_COMM_NO;                    // 시리얼 통신 번호
            var intRecircCnt        = _item.RECIRC_CNT;                     // 순환횟수
            var strZoneId           = _item.ZONE_ID;                        // Zone Id
            var strUseYN            = _item.Checked == true ? "Y" : "N";    // 사용 여부
            var intMaxReadCnt       = _item.MAX_READ_CNT;                   // 최대 리딩 횟수
            var intRecirculation    = _item.MAX_RECIRCULATION;              // 최대 회전 횟수
            var intMaxScanCnt       = _item.MAX_SCAN_CNT;                   // 최대 바코드 인식 수
            var strIsRunYn          = _item.IS_RUN_YN;                      // 운영상태
            var strSortPlnCd        = _item.SORT_PLN_CD;                    // 플랜코드
            var strUserID           = this.BaseClass.UserID;                // 사용자 ID
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("EQP_ID", strEqpID);                          // 설비 ID
            dicInputParam.Add("EQP_NM", strEqpNm);                          // 설비 명
            dicInputParam.Add("EQP_DESC", strEqpDesc);                      // 설비 세부 정보
            dicInputParam.Add("EQP_TYPE_CD", strEqpTypeCd);                 // 설비 종류 코드
            dicInputParam.Add("LINK_EQP_ID", strLinkEqpId);                 // 연결설비ID
            dicInputParam.Add("LOC_CD", strLocCd);                          // 위치 코드
            dicInputParam.Add("PC_IP", strPcIp);                            // 설비와 통신하는 PC IP
            dicInputParam.Add("ECS_COMM_NO", strEcsCommNo);                 // ECS 통신번호
            dicInputParam.Add("SER_COMM_NO", strSerCommNo);                 // 시리얼 통신 번호
            dicInputParam.Add("RECIRC_CNT", intRecircCnt);                  // 순환횟수
            dicInputParam.Add("ZONE_ID", strZoneId);                        // Zone Id
            dicInputParam.Add("USE_YN", strUseYN);                          // 사용 여부
            dicInputParam.Add("MAX_READ_CNT", intMaxReadCnt);               // 최대 리딩 횟수
            dicInputParam.Add("MAX_RECIRCULATION", intRecirculation);       // 최대 회전 횟수
            dicInputParam.Add("MAX_SCAN_CNT", intMaxScanCnt);               // 최대 바코드 인식 수
            dicInputParam.Add("IS_RUN_YN", strIsRunYn);                     // 운영상태
            dicInputParam.Add("SORT_PLN_CD", strSortPlnCd);                 // 플랜코드
            dicInputParam.Add("USER_ID", strUserID);                        // 사용자 ID
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            }).ConfigureAwait(true);
            #endregion

            if (dicRtnValue["RTN_VAL"].ToString().Equals("0") == false)
            {
                var strMessage = dicRtnValue["RTN_MSG"].ToString();
                this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_EQP_UPD - Equipment 수정
        /// <summary>
        /// Equipment 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_EQP_UPD(BaseDataAccess _da, EquipmentMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "UI_EQIP_MST_UPD";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            Dictionary<object, BaseEnumClass.MSSqlOutputDataType> dicOutPutParam = new Dictionary<object, BaseEnumClass.MSSqlOutputDataType>();
            Dictionary<object, object> dicRtnValue = new Dictionary<object, object>();

            var strEqpID                = _item.EQP_ID;                             // 설비 ID
            var strEqpNm                = _item.EQP_NM;                             // 설비 명
            var strEqpDesc              = _item.EQP_DESC;                           // 설비 세부 정보
            var strEqpTypeCd            = _item.EQP_TYPE_CD;                        // 설비 종류
            var strLinkEqpId            = _item.LINK_EQP_ID;                        // 연결설비ID
            var strLocCd                = _item.LOC_CD;                             // 위치 코드
            var strPcIp                 = _item.PC_IP;                              // 설비랑 통신하는 PC IP
            var strEcsCommNo            = _item.ECS_COMM_NO;                        // 설비 ECS 통신 번호
            var strSerCommNo            = _item.SER_COMM_NO;                        // 시리얼 통신 번호
            var intRecircCnt            = _item.RECIRC_CNT;                         // 순환횟수
            var strZoneId               = _item.ZONE_ID;                            // ZONE ID
            var strUseYN                = _item.Checked == true ? "Y" : "N";        // 사용 여부
            var intMaxReadCnt           = _item.MAX_READ_CNT;                       // 최대 리딩 횟수
            var intMaxRecirculation     = _item.MAX_RECIRCULATION;                  // 최대 회전 횟수
            var intMaxScanCnt           = _item.MAX_SCAN_CNT;                       // 최대 바코드 인식 수
            var strIsRunYn              = _item.IS_RUN_YN;                          // 운영상태
            var strSortPlnCd            = _item.SORT_PLN_CD;                        // 플랜코드
            var strUserID               = this.BaseClass.UserID;                    // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("EQP_ID",             strEqpID);                      // 설비 ID
            dicInputParam.Add("EQP_NM",             strEqpNm);                      // 설비 명
            dicInputParam.Add("EQP_DESC",           strEqpDesc);                    // 설비 세부 정보
            dicInputParam.Add("EQP_TYPE_CD",        strEqpTypeCd);                  // 설비 종류
            dicInputParam.Add("LINK_EQP_ID",        strLinkEqpId);                  // 연결설비ID
            dicInputParam.Add("LOC_CD",             strLocCd);                      // 위치 코드
            dicInputParam.Add("PC_IP",              strPcIp);                       // 설비랑 통신하는 PC IP
            dicInputParam.Add("ECS_COMM_NO",        strEcsCommNo);                  // 설비 ECS 통신 번호
            dicInputParam.Add("SER_COMM_NO",        strSerCommNo);                  // 시리얼 통신 번호
            dicInputParam.Add("RECIRC_CNT",         intRecircCnt);                  // 순환횟수
            dicInputParam.Add("ZONE_ID",            strZoneId);                     // ZONE ID
            dicInputParam.Add("USE_YN",             strUseYN);                      // 사용 여부
            dicInputParam.Add("MAX_READ_CNT",       intMaxReadCnt);                 // 최대 리딩 횟수
            dicInputParam.Add("MAX_RECIRCULATION",  intMaxRecirculation);           // 최대 회전 횟수
            dicInputParam.Add("MAX_SCAN_CNT",       intMaxScanCnt);                 // 최대 바코드 인식 수
            dicInputParam.Add("IS_RUN_YN",          strIsRunYn);                    // 운영상태
            dicInputParam.Add("SORT_PLN_CD",        strSortPlnCd);                  // 플랜코드
            dicInputParam.Add("P_USER_ID",          strUserID);                     // 사용자 ID
            #endregion

            #region + Output 파라메터
            dicOutPutParam.Add("RTN_VAL", BaseEnumClass.MSSqlOutputDataType.INT32);
            dicOutPutParam.Add("RTN_MSG", BaseEnumClass.MSSqlOutputDataType.VARCHAR);
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, dicOutPutParam, ref dicRtnValue);
            }).ConfigureAwait(true);
            #endregion

            if (dicRtnValue["RTN_VAL"].ToString().Equals("0") == false)
            {
                var strMessage = dicRtnValue["RTN_MSG"].ToString();
                this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion
        #endregion
        
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void M1002_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= M1002_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region > UnLoaded 이벤트
        private void M1002_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Unloaded -= M1002_Unloaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 설비 관리
        #region >> 버튼 클릭 이벤트
        #region + 설비 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangedRowData = this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    EquipmentListSearch();
                }
            }
            else
            {
                EquipmentListSearch();
            }
        }
        #endregion

        #region + 설비 관리 조회
        /// <summary>
        /// 설비 관리 조회
        /// </summary>
        private void EquipmentListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_EQP_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg, BaseEnumClass.SelectedDatabaseKind.MS_SQL) == true)
                {
                    // 정상 처리된 경우
                    this.EquipmentMgntList = new ObservableCollection<EquipmentMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.EquipmentMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.EquipmentMgntList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.EquipmentMgntList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
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

        #region + 설비 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 엑셀 다운로드 버튼 클릭 이벤트
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

        #region + 설비 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.EquipmentMgntList.ForEach(p => p.ClearError());

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                foreach (var item in this.EquipmentMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.EQP_ID) == true)
                        {
                            item.CellError("EQP_ID", string.Format(strMessage, this.BaseClass.GetResourceValue("EQP_ID")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_EQP_INS(da, item).Result;
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_EQP_UPD(da, item).Result;
                            }
                            
                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();
                            
                            BaseClass.MsgInfo("CMPT_SAVE");

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                            }

                            // 저장 후 저장내용 List에 출력 : Header
                            DataSet dsRtnValue = this.GetSP_EQP_LIST_INQ();

                            this.EquipmentMgntList = new ObservableCollection<EquipmentMgnt>();
                            this.EquipmentMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.EquipmentMgntList;
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_MSSQL == BaseEnumClass.TransactionState_MSSQL.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;

                        //// 체크박스 해제
                        //foreach (var item in liSelectedRowData)
                        //{
                        //    item.IsSelected = false;
                        //}

                        //// 저장 후 저장내용 List에 출력 : Header
                        //DataSet dsRtnValue = this.GetSP_EQP_LIST_INQ();

                        //this.EquipmentMgntList = new ObservableCollection<EquipmentMgnt>();
                        //this.EquipmentMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                        //this.gridMaster.ItemsSource = this.EquipmentMgntList;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new EquipmentMgnt
            {
                EQP_ID              = string.Empty,
                EQP_NM              = string.Empty,
                EQP_DESC            = string.Empty,
                EQP_TYPE_CD         = string.Empty,
                LINK_EQP_ID         = string.Empty,
                LOC_CD              = string.Empty,
                USE_YN              = "Y",
                PC_IP               = string.Empty,
                ECS_COMM_NO         = string.Empty,
                SER_COMM_NO         = string.Empty,
                RECIRC_CNT          = 0,
                ZONE_ID             = string.Empty,
                MAX_READ_CNT        = 0,
                MAX_RECIRCULATION   = 0,
                MAX_SCAN_CNT        = 0,
                IS_RUN_YN           = "N",
                SORT_PLN_CD         = string.Empty,
                IsSelected          = true,
                IsNew               = true,
                IsSaved             = false
            };

            this.EquipmentMgntList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn           = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle   = this.EquipmentMgntList.Count - 1;

            this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
            this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

            //var aaa = this.tvMasterGrid_First.FocusedRowHandle;
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }

            this.DeleteGridRowItem();
        }
        #endregion
        #endregion

        #region >> 그리드 관련 이벤트
        #region + 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.InRowCell)
            {
                if (hi.Column.FieldName.Equals("EQP_ID")) { return; }

                if (view.ActiveEditor == null)
                {
                    view.ShowEditor();

                    if (view.ActiveEditor == null) { return; }
                    Dispatcher.BeginInvoke(new Action(() => {
                        view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                    }), DispatcherPriority.Render);
                }
            }
        }
        #endregion

        #region + 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// <summary>
        /// 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData == true)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid") == true)
            {
                EquipmentMgnt dataMember = tv.Grid.CurrentItem as EquipmentMgnt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 설비 ID, 설비 명 컬럼은 수정이 되지 않도록 처리한다.
                    case "EQP_ID":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
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