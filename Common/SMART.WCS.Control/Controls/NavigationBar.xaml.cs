using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.Control
{
    /// <summary>
    /// 업무화면 네비게이션
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        #region ▩ Detegate 선언
        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void UserControlCallEventHandler();
        public event UserControlCallEventHandler UserControlCallEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자
        public NavigationBar()
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();
        }

        #region  ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ItemsSource != null)
            {
                SetMenuNavigationBarValue(ItemsSource);
            }
        }
        #endregion
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public static readonly DependencyProperty itemsSourceProperty 
            = DependencyProperty.Register("ItemsSource", typeof(List<string>)
                , typeof(NavigationBar), new PropertyMetadata(new List<string>()));

        /// <summary>
        /// Navigation 데이터 바인딩용 ItemsSource
        /// </summary>
        public List<string> ItemsSource
        {
            get { return (List<string>)GetValue(itemsSourceProperty); }
            set { SetValue(itemsSourceProperty, value); }
        }


        public static readonly DependencyProperty MenuIDProperty
            = DependencyProperty.Register("MenuID", typeof(string)
                , typeof(NavigationBar), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 메뉴 ID (클래스명)
        /// </summary>
        public string MenuID
        {
            get { return (string)GetValue(MenuIDProperty); }
            set { SetValue(MenuIDProperty, value); }
        }

        /// <summary>
        /// ForuthLevel 속성
        /// </summary>
        public static readonly DependencyProperty FourthLevelProperty = DependencyProperty.Register("FourthLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ForuthLevel
        /// </summary>
        public string FourthLevel
        {
            get { return (string)GetValue(FourthLevelProperty); }
            set { SetValue(FourthLevelProperty, value); }
        }

        /// <summary>
        /// ThirdLevel 속성
        /// </summary>
        public static readonly DependencyProperty ThirdLevelProperty = DependencyProperty.Register("ThirdLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ThirdLevel
        /// </summary>
        public string ThirdLevel
        {
            get { return (string)GetValue(ThirdLevelProperty); }
            set { SetValue(ThirdLevelProperty, value); }
        }

        /// <summary>
        /// ThirdLevel 속성
        /// </summary>
        public static readonly DependencyProperty SecondLevelProperty = DependencyProperty.Register("SecondLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ThirdLevel
        /// </summary>
        public string SecondLevel
        {
            get { return (string)GetValue(SecondLevelProperty); }
            set { SetValue(SecondLevelProperty, value); }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            // 화면 로드 이벤트
            this.Loaded += NavigationBar_Loaded;
        }
        #endregion
        #endregion

        #region > 기타
        #region >> SetMenuNavigationBarValue - 업무화면 메뉴 네비게이션 값을 설정한다.
        /// <summary>
        /// 업무화면 메뉴 네비게이션 값을 설정한다.
        /// </summary>
        /// <param name="_gridNavigation">시스템 그리드 컨트롤</param>
        /// <param name="_liMenuNavigation">메뉴 데이터</param>
        public void SetMenuNavigationBarValue(List<string> _liMenuNavigation)
        {
            try
            {
                // 메뉴 레벨수
                int iMenuDepth = _liMenuNavigation.Count();

                #region TextBlock 컨트롤을 찾아 Name을 저장한다.
                // TextBlock를 싸고 있는 Grid 태그를 중심으로 컨트롤을 구한다.
                for (int i = 0; i < _gridNavigation.Children.Count; i++)
                //for (int i = _gridNavigation.Children.Count - 1; i > 0; i--)
                {
                    // 자식 컨트롤을 저장한다.
                    var childControl = _gridNavigation.Children[i];

                    // 해당 컨트롤 찾기 (FindControl)
                    if (childControl is System.Windows.Controls.TextBlock)
                    {
                        System.Windows.Controls.TextBlock textBlock = (System.Windows.Controls.TextBlock)childControl;

                        if (iMenuDepth == 1)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFirstChar":
                                case "lblSecondNavigation":
                                case "lblSecondChar":
                                case "lblThirdNavigation":
                                case "lblThirdChar":
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 2)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblSecondChar":
                                case "lblThirdNavigation":
                                case "lblThirdChar":
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 3)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                case "lblThirdChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                case "lblSecondChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblThirdNavigation":
                                    textBlock.Text = _liMenuNavigation[2];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 4)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                case "lblSecondChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblThirdNavigation":
                                    textBlock.Text = _liMenuNavigation[2];
                                    break;
                                case "lblThirdChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblFourthNavigation":
                                    textBlock.Text = _liMenuNavigation[3];
                                    break;
                                case "lblFourthChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                default: break;
                            }
                        }
                    }
                }
                #endregion
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #endregion

        #region ▩ 이벤트
        #region > 화면 로드 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NavigationBar_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true) { return; }

                this.g_isLoaded = false;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #endregion
    }
}
