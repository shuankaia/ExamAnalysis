using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace AnalysisExams.ViewModel
{
    public class Answer
    {
        public string Content { get; set; }
        public bool Correct { get; set; }
        public Answer(string content, bool correct)
        {
            Content = content;
            Correct = correct;
        }
        public Answer()
        {

        }
    }
    public class Answer2
    {
        public string Content { get; set; }
        public bool Correct { get; set; }
        public SolidColorBrush Color { get; set; } = Brushes.Black;
        public Answer2(Answer answer)
        {
            Content = answer.Content;
            Correct = answer.Correct;
        }
    }
    public class UserAnswer
    {
        //public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();
        public ObservableCollection<Answer2> Answers { get; set; } = new ObservableCollection<Answer2>();
        public bool Wrong { get; set; }
        public UserAnswer(ObservableCollection<Answer> answers, ObservableCollection<Answer> correctAnswers)
        {
            foreach (var item in answers)
            {
                Answers.Add(new Answer2(item));
            }
            for (int i = 0; i < Answers.Count; i++)
            {
                if(Answers[i].Correct != correctAnswers[i].Correct)
                {
                    if (correctAnswers[i].Correct)
                    {
                        Answers[i].Color = Brushes.Blue;
                    }
                    else
                    {
                        Answers[i].Color = Brushes.Red;
                    }
                }
                else if(correctAnswers[i].Correct)
                {
                    Answers[i].Color = Brushes.Green;
                }
                else
                {
                    Answers[i].Color = Brushes.Black;
                }
            }
        }
        //public UserAnswer(ObservableCollection<Answer> answers)
        //{
        //    Answers = answers;
        //}
    }
    public class Problem
    {
        public ProblemType ProblemType { get; set; }
        public int Index { get; set; }
        public string Question { get; set; }
        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();
        public ObservableCollection<UserAnswer> UserAnswers { get; set; } = new ObservableCollection<UserAnswer>();
        public int Wrong { get; set; } = 0;
        public Problem(int index, ProblemType problemType, string question, ObservableCollection<Answer> answers)
        {
            Index = index;
            ProblemType = problemType;
            Question = question;
            Answers = answers;
        }
        public string GetCorrectAnswer()
        {
            string ans = "\t" + "参考答案 : ";
            int n = 0;
            foreach (var answer in Answers)
            {
                if (answer.Correct)
                {
                    n++;
                    if (n > 1) ans += "┋";
                    ans += answer.Content; 
                }
            }
            return ans;
        }
        public string GetWjxCorrectAnswer()
        {
            string ans = "(";
            int n = 0;
            foreach (var answer in Answers)
            {
                if (answer.Correct)
                {
                    ans += answer.Content[0];
                }
            }
            ans +=")";
            return ans ;
        }
        public string GetCustomAnswer()
        {
            string ans = "[";
            int n = 0;
            foreach (var answer in Answers)
            {
                ans += $@"{{answer:'{ answer.Content}',right:{answer.Correct.ToString().ToLower()}}},";
                //ans += string.Format("{answer:\"{0}\",right:{1}},", answer.Content[0], answer.Correct);
            }
            ans += "]";
            return ans;
        }
        public string GetWjxAnswer()
        {
            string ans = "";
            foreach (var answer in Answers)
            {
                ans += answer.Content + Environment.NewLine;
            }
            return ans;
        }
        public override string ToString()
        {
            string ans = "";
            foreach (var answer in Answers)
            {
                ans += Environment.NewLine + "\t" + answer.Content + Environment.NewLine;
            }
            return Question + System.Environment.NewLine + ans;
        }
    }

    public class Paper : ViewModelBase
    {
        private ObservableCollection<Problem> _problems = new ObservableCollection<Problem>();
        private int _importNum = 0;
        private int _importValidNum = 0;
        private int _maxIndex = 0;
        private int _wrongNum = 0;
        private int _wrongKind;
        public string AllHtml { get; set; }
        public ObservableCollection<Problem> Problems
        {
            get { return _problems; }
            set { _problems = value; RaisePropertyChanged<ObservableCollection<Problem>>(nameof(Problems)); }
        }
        public string ToMd(bool withAnswer = false)
        {
            string md = "";
            int index = 0;
            foreach (var problem in Problems)
            {
                index++;
                md += index.ToString()+". " + problem.ToString() + Environment.NewLine;
                if (withAnswer)
                {
                    md += problem.GetCorrectAnswer() + Environment.NewLine + Environment.NewLine;
                }

            }

            return md;
        }
        public string ToWjx()
        {
            string md = "";
            int index = 0;
            foreach (var problem in Problems)
            {
                index++;
                string bs="[单选题]";
                switch (problem.ProblemType)
                {
                    case ProblemType.Radio:
                        bs = "[单选题]"; break;
                    case ProblemType.MultiSelect:
                        bs = "[多选题]"; break;
                    case ProblemType.Judge:
                        bs = "[判断题]"; break;
                    default:
                        break;
                }
                string ans = problem.GetWjxCorrectAnswer();

                md += index.ToString() + "、 " + problem.Question+ bs+ans+ Environment.NewLine;
                md += problem.GetWjxAnswer() + Environment.NewLine;


            }

            return md;
        }
        public string ToCustom()
        {
            string md = "let questions=[";
            int index = 0;
            foreach (var problem in Problems)
            {
                index++;
                string type = "single";
                string value = "10";
                switch (problem.ProblemType)
                {
                    case ProblemType.Radio:
                        type = "single"; value = "1"; break;
                    case ProblemType.MultiSelect:
                        type = "multiple"; value = "2"; break;
                    case ProblemType.Judge:
                        type = "single"; value = "1"; break;
                    default:
                        break;
                }
                string ans = problem.GetCustomAnswer();
                string ps = $@"{{type:'{type}', value:{value},question:'{problem.Question}', answers:{ans}}}";

                md += ps + ",";
            }
            md += "]";
            return md;
        }
        public int ImportNum
        {
            get { return _importNum; }
            set { _importNum = value; RaisePropertyChanged<int>(nameof(ImportNum)); }
        }
        public int ImportValidNum
        {
            get { return _importValidNum; }
            set { _importValidNum = value; RaisePropertyChanged<int>(nameof(ImportValidNum)); }
        }
        public int MaxIndex
        {
            get { return _maxIndex; }
            set { _maxIndex = value; RaisePropertyChanged<int>(nameof(MaxIndex)); }
        }
        public int WrongNum
        {
            get { return _wrongNum; }
            set { _wrongNum = value; RaisePropertyChanged<int>(nameof(WrongNum)); }
        }
        public int WrongKind
        {
            get { return _wrongKind; }
            set { _wrongKind = value; RaisePropertyChanged<int>(nameof(WrongKind)); }
        }
    }
    public enum ProblemType
    {
        Radio,
        MultiSelect,
        Judge,
    }
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<T> Convert<T>(List<T> source)
        {
            ObservableCollection<T> to = new ObservableCollection<T>(source);
            //或者source.ForEach(p => to.Add(p));
            return to;
        }

        public List<T> Convert<T>(ObservableCollection<T> source)
        {
            List<T> to = new List<T>(source);
            return to;
        }

        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public RelayCommand ImportDataCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand SortByWrongNumCommand { get; set; }
        public RelayCommand SortByIndexCommand { get; set; }
        public RelayCommand OnlyRadioCommand { get; set; }
        public RelayCommand OnlyMultiSelectCommand { get; set; }
        public RelayCommand OnlyJudgeCommand { get; set; }
        public RelayCommand ReflashAllCommand { get; set; }
        public RelayCommand OnlyShowWrongCommand { get; set; }
        public RelayCommand ExportWithCorrectCommand { get; set; }
        public RelayCommand ExportWithWrongCommand { get; set; }
        public RelayCommand ExportWJXCommand { get; set; }
        public RelayCommand ExportCustomCommand { get; set; }
        public RelayCommand SortRandomCommand { get; set; }
        private Paper _paper = new Paper();

        public Paper Paper
        {
            get { return _paper; }
            set { _paper = value; RaisePropertyChanged<Paper>(nameof(Paper)); }
        }


        Comparison<Problem> comparisonIndex = new Comparison<Problem>((p1, p2) => {
            if (p1.Index < p2.Index) return -1;
            else if (p1.Index > p2.Index) return 1;
            else return 0;
        });
        Comparison<Problem> comparisonWrong = new Comparison<Problem>((p1, p2) => {
            if (p1.Wrong > p2.Wrong) return -1;
            else if (p1.Wrong < p2.Wrong) return 1;
            else return 0;
        });
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        private List<Problem> ListRandom(List<Problem> myList)
        {

            Random ran = new Random();
            int index = 0;
            Problem temp = null;
            for (int i = 0; i < myList.Count; i++)
            {

                index = ran.Next(0, myList.Count - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                ImportDataCommand = new RelayCommand(() => {
                    OpenFileDialog dlg = new OpenFileDialog();
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        StreamReader sr = new StreamReader(dlg.FileName);
                        string html = sr.ReadToEnd();
                        sr.Close();
                        Paper.AllHtml += html;
                        ImportData(html);
                    //this.Paper.Problems.OrderBy(m=>m.Index);

                    }

                });
                ClearCommand = new RelayCommand(() => {
                    this.Paper = new Paper();
                });
                SortByWrongNumCommand = new RelayCommand(() =>
                {
                    var a = Convert(this.Paper.Problems);
                    a.Sort(comparisonWrong);
                    this.Paper.Problems = Convert(a);
                });
                SortByIndexCommand = new RelayCommand(() =>
                {
                    var a = Convert(this.Paper.Problems);
                    a.Sort(comparisonIndex);
                    this.Paper.Problems = Convert(a);
                });
                SortRandomCommand = new RelayCommand(() => {
                    var a = Convert(this.Paper.Problems);
                    a = ListRandom(a);
                    this.Paper.Problems = Convert(a);
                });
                OnlyRadioCommand = new RelayCommand(() =>
                {
                    var np = new Paper();
                    np.AllHtml = this.Paper.AllHtml;
                    this.Paper = np;
                    ImportData(this.Paper.AllHtml);
                    var a = Convert(this.Paper.Problems);
                    a = a.FindAll(m=>m.ProblemType == ProblemType.Radio);
                    this.Paper.Problems = Convert(a);
                });
                OnlyMultiSelectCommand = new RelayCommand(() =>
                {
                    var np = new Paper();
                    np.AllHtml = this.Paper.AllHtml;
                    this.Paper = np;
                    ImportData(this.Paper.AllHtml);
                    var a = Convert(this.Paper.Problems);
                    a = a.FindAll(m => m.ProblemType == ProblemType.MultiSelect);
                    this.Paper.Problems = Convert(a);
                });
                OnlyJudgeCommand = new RelayCommand(() =>
                {
                    var np = new Paper();
                    np.AllHtml = this.Paper.AllHtml;
                    this.Paper = np;
                    ImportData(this.Paper.AllHtml);
                    var a = Convert(this.Paper.Problems);
                    a = a.FindAll(m => m.ProblemType == ProblemType.Judge);
                    this.Paper.Problems = Convert(a);
                });
                ReflashAllCommand = new RelayCommand(() =>
                {
                    var np = new Paper();
                    np.AllHtml = this.Paper.AllHtml;
                    this.Paper = np;
                    ImportData(this.Paper.AllHtml);
                });
                OnlyShowWrongCommand = new RelayCommand(() =>
                {
                    var a = Convert(this.Paper.Problems);
                    a = a.FindAll(m => m.Wrong > 0);
                    this.Paper.Problems = Convert(a);
                });
                ExportCommand = new RelayCommand(() => {
                    //可能要获取的路径名
                    string localFilePath = "", fileNameExt = "", newFileName = "", FilePath = "";
                    SaveFileDialog saveFileDialog = new SaveFileDialog();


                    //设置文件类型
                    //书写规则例如：txt files(*.txt)|*.txt
                    saveFileDialog.Filter = "md files(*.md)|*.md|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                    //设置默认文件名（可以不设置）
                    saveFileDialog.FileName = "Exam";
                    //主设置默认文件extension（可以不设置）
                    saveFileDialog.DefaultExt = "md";
                    //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                    saveFileDialog.AddExtension = true;

                    //设置默认文件类型显示顺序（可以不设置）
                    saveFileDialog.FilterIndex = 1;

                    //保存对话框是否记忆上次打开的目录
                    saveFileDialog.RestoreDirectory = true;

                    // Show save file dialog box
                    DialogResult result = saveFileDialog.ShowDialog();
                    //点了保存按钮进入
                    if (result == DialogResult.OK)
                    {
                        //获得文件路径
                        localFilePath = saveFileDialog.FileName.ToString();
                        System.IO.File.WriteAllText(localFilePath, this.Paper.ToMd());

                    }
                });
                ExportWithCorrectCommand = new RelayCommand(() => {
                    //可能要获取的路径名
                    string localFilePath = "", fileNameExt = "", newFileName = "", FilePath = "";
                    SaveFileDialog saveFileDialog = new SaveFileDialog();


                    //设置文件类型
                    //书写规则例如：txt files(*.txt)|*.txt
                    saveFileDialog.Filter = "md files(*.md)|*.md|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                    //设置默认文件名（可以不设置）
                    saveFileDialog.FileName = "Exam";
                    //主设置默认文件extension（可以不设置）
                    saveFileDialog.DefaultExt = "md";
                    //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                    saveFileDialog.AddExtension = true;

                    //设置默认文件类型显示顺序（可以不设置）
                    saveFileDialog.FilterIndex = 1;

                    //保存对话框是否记忆上次打开的目录
                    saveFileDialog.RestoreDirectory = true;

                    // Show save file dialog box
                    DialogResult result = saveFileDialog.ShowDialog();
                    //点了保存按钮进入
                    if (result == DialogResult.OK)
                    {
                        //获得文件路径
                        localFilePath = saveFileDialog.FileName.ToString();
                        string re = this.Paper.ToMd(true);
                        System.IO.File.WriteAllText(localFilePath, re);
                    }
                });
                ExportWithWrongCommand = new RelayCommand(() => { 

                });
                ExportWJXCommand = new RelayCommand(() =>
                {
                    //可能要获取的路径名
                    string localFilePath = "", fileNameExt = "", newFileName = "", FilePath = "";
                    SaveFileDialog saveFileDialog = new SaveFileDialog();


                    //设置文件类型
                    //书写规则例如：txt files(*.txt)|*.txt
                    saveFileDialog.Filter = "md files(*.md)|*.md|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                    //设置默认文件名（可以不设置）
                    saveFileDialog.FileName = "Exam";
                    //主设置默认文件extension（可以不设置）
                    saveFileDialog.DefaultExt = "md";
                    //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                    saveFileDialog.AddExtension = true;

                    //设置默认文件类型显示顺序（可以不设置）
                    saveFileDialog.FilterIndex = 2;

                    //保存对话框是否记忆上次打开的目录
                    saveFileDialog.RestoreDirectory = true;

                    // Show save file dialog box
                    DialogResult result = saveFileDialog.ShowDialog();
                    //点了保存按钮进入
                    if (result == DialogResult.OK)
                    {
                        //获得文件路径
                        localFilePath = saveFileDialog.FileName.ToString();
                        string re = this.Paper.ToWjx();
                        System.IO.File.WriteAllText(localFilePath, re);
                    }
                });
                ExportCustomCommand = new RelayCommand(() => {
                    //可能要获取的路径名
                    string localFilePath = "", fileNameExt = "", newFileName = "", FilePath = "";
                    SaveFileDialog saveFileDialog = new SaveFileDialog();


                    //设置文件类型
                    //书写规则例如：txt files(*.txt)|*.txt
                    saveFileDialog.Filter = "html files(*.html)|*.html|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                    //设置默认文件名（可以不设置）
                    saveFileDialog.FileName = "index";
                    //主设置默认文件extension（可以不设置）
                    saveFileDialog.DefaultExt = "html";
                    //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                    saveFileDialog.AddExtension = true;

                    //设置默认文件类型显示顺序（可以不设置）
                    saveFileDialog.FilterIndex = 1;

                    //保存对话框是否记忆上次打开的目录
                    saveFileDialog.RestoreDirectory = true;

                    // Show save file dialog box
                    DialogResult result = saveFileDialog.ShowDialog();
                    //点了保存按钮进入
                    if (result == DialogResult.OK)
                    {
                        //获得文件路径
                        localFilePath = saveFileDialog.FileName.ToString();
                        string re = this.Paper.ToCustom();
                        re = Properties.Resources.index_html.Replace("\"(####Problem####)\"", re);
                        System.IO.File.WriteAllText(localFilePath, re);
                    }
                });
            }
        }
        public void ImportData(string html)
        {
            html = html.Replace(@"<u>&nbsp; &nbsp; &nbsp; &nbsp;</u>", "_____")
                .Replace("<u>&nbsp; &nbsp; &nbsp; &nbsp; </u>", "_____")
                .Replace("<u>&nbsp; &nbsp; &nbsp; </u>", "_____");// 处理特殊情况
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNodeCollection unitNodes = doc.DocumentNode.SelectNodes("//div[@class='query__data-result new__data-result']//div[@class='data__items']");
            if (unitNodes == null || unitNodes.Count == 0) return;// 无数据不处理
            foreach (var unit in unitNodes)
            {
                this.Paper.ImportNum++;
                var questionRealId = int.Parse(unit.Attributes["topic"].Value);
                if (questionRealId > this.Paper.MaxIndex) this.Paper.MaxIndex = questionRealId;
                var questionNode = unit.SelectSingleNode("./div[@class='data__tit_cjd']");
                //var questionId = questionNode.ChildNodes[0].InnerText.Trim();
                var questionStr = Regex.Replace(questionNode.ChildNodes[1].InnerText.Replace("&nbsp;", "").Replace(" ", ""), @"\s+", " ");
                var optionNodes = unit.SelectNodes("./div[@class='data__key']//div[@class='ulradiocheck']//div");
                var correctNode = unit.SelectSingleNode("./div[@class='data__key']//div[@class='judge_ques_right']");
                ObservableCollection<Answer> answerList = new ObservableCollection<Answer>();
                ObservableCollection<Answer> userAnswer = new ObservableCollection<Answer>();
                // 判断问题类型
                ProblemType problemType = ProblemType.Radio;
                var iconflag = optionNodes[0].SelectSingleNode("./i").InnerText;
                if (iconflag == "&#xe6e1;" || iconflag == "&#xe6e2;") problemType = ProblemType.MultiSelect;// 多选标志
                else
                {
                    if (optionNodes.Count == 2) problemType = ProblemType.Judge;
                }
                // 提取选项
                foreach (var option in optionNodes)
                {
                    var optionContentNode = option.SelectSingleNode("./span");
                    var optionIconNode = option.SelectSingleNode("./i");
                    var optionContent = Regex.Replace(optionContentNode.InnerText, @"\s+", " ");
                    var optionIcon = optionIconNode.InnerText;
                    bool correct = false;
                    if (optionIcon == "&#xe6df;" || optionIcon == "&#xe6e1;") correct = true;
                    else if (optionIcon == "&#xe6e0;" || optionIcon == "&#xe6e2;") correct = false;
                    else { throw new Exception("unknow info"); }
                    userAnswer.Add(new Answer(optionContent, correct));
                }

                // 处理正确答案提取
                answerList = DeepCopy(userAnswer);// 
                if (correctNode == null)// 没有显示正确提示，那就是错误的，对拷贝来的用户答案进行修正
                {
                    this.Paper.WrongNum++;
                    var answerNode = unit.SelectSingleNode("./div[@class='data__key']//div[@class='answer-ansys']//div");
                    //Console.WriteLine(answerNode.InnerText);

                    switch (problemType)
                    {
                        case ProblemType.Radio:
                        case ProblemType.Judge:
                            answerList.ToList().ForEach(m => {
                                if (m.Content.Replace(" ","") == answerNode.InnerText.Replace(" ", ""))
                                {
                                    m.Correct = true;
                                }
                                else
                                {
                                    m.Correct = false;
                                }
                            });
                            break;
                        case ProblemType.MultiSelect:
                            var ma = answerNode.InnerText.Split('┋');
                            answerList.ToList().ForEach(m => { m.Correct = false; });
                            foreach (var ima in ma)
                            {
                                answerList.ToList().ForEach(m => {
                                    if (m.Content.Replace(" ", "") == ima.Replace(" ", ""))
                                    {
                                        m.Correct = true;
                                    }
                                });
                            }
                            break;

                        default:
                            break;
                    }
                }
                
                Problem problem = Convert(this.Paper.Problems).Find(m => m.Index == questionRealId);
                if (problem == null)
                {
                    problem = new Problem(questionRealId, problemType, questionStr, answerList);
                    this.Paper.Problems.Add(problem);
                }
                problem.UserAnswers.Add(new UserAnswer(userAnswer, answerList));
                if (correctNode == null) problem.Wrong++;
            }
            this.Paper.ImportValidNum = this.Paper.Problems.Count;
            this.Paper.WrongKind = Convert(this.Paper.Problems).FindAll(m => m.Wrong != 0).Count;
        }
    }
}