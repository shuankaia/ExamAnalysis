   M S B A M L   `   `   `  ���� H  DAnalysisExams, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:AnalysisExamsAnalysisExams  A%clr-namespace:AnalysisExams.ConverterAnalysisExams.Converter      AnalysisExams.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:AnalysisExams     ;	converter%clr-namespace:AnalysisExams.Converter       � Title$  
MainWindow��$	��450��$	��800�� � DataContext ��  �� Path$ MainViewModel�   Locator �� Source# [   � 	Resources 2   ,AnalysisExams.Converter.BoolToColorConverter  BoolToColorConverter��%   &	         .��� $��5q� � RowDefinitions �� $��* ��� $��8* ��� $��* �.+��� $��0�� �� Orientation$ 
Horizontal=�.+��� $��50��$��导入��$��5q����� *ImportDataCommand+�� $��50��$��清空��$��5q����� *ClearCommand+�� $��50��$��只看单选��$��5q����� *OnlyRadioCommand+�� $��50��$��只看多选��$��5q����� *OnlyMultiSelectCommand+�� $��50��$��只看判断��$��5q����� *OnlyJudgeCommand+�� $��50��$��加载所有��$��5q����� *ReflashAllCommand+�� $��30���� $��50��$��导出题库��$��5q����� *ExportCommand+�� $��50��$��导出题库(答案)��$��5q����� *ExportWithCorrectCommand+�� $��50��$#��导出题库(含错误情况)��$��5q����� *ExportWithWrongCommand+�� $��50��$��导出试卷(问卷星)��$��5q����� *ExportWJXCommand+�� $��50��$��导出试卷(自定义)��$��5q����� *ExportCustomCommand+�� $��30���� $��50��$��随机排序��$��5q����� *SortRandomCommand+�� $��50��$��按错误数量排序��$��5q����� *SortByWrongNumCommand+�� $��50��$��按序号排序��$��5q����� *SortByIndexCommand+�� $��50��$��当前模式只看错题��$��5q����� *OnlyShowWrongCommand+&   System.Windows.Controls.DataGrid  $��1��   AutoGenerateColumns .  ���� *Paper.Problems+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn	   Header$	 题号��1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 �� *Index+4  .System.Windows.Controls.DataGridTemplateColumn  $	 题目��   MaxWidth$	 300Q�   CellTemplate �� .R��� $��18(�$
��Wrap=����� *
Question+  $	 答案�� �� .R�  $
��Left=�   CanUserAddRows .     HeadersVisibility$
 None=� .  ���� *	Answers+   $	  �� �� .R��� $��16(�$
��Left=�$	��200��$
��Wrap=����� *	Content+���� *	Correct+ �� 	Converter# [    �� .R�  $
 None=�$
��Left=� .  ���� *UserAnswers+   	CellStyle ��  �� 
TargetType*  $System.Windows.Controls.DataGridCell"  .���  �� Property �@�LayoutTransform �� Value V� .~���  �� ScaleX$ 1Q� �� ScaleY$ -1Q���  �� Angle$ 90Q� � LayoutTransform V� .~��� $	 -90Q��� $ 1Q�$ -1Q�    �� .R�  $
��Left=� .  $
 None=� .  ���� *	Answers+   $	  �� �� .R��� $��16(�$
��Left=�$	��200��$
��Wrap=����� *	Content+���� *Color+�� $��2��$ 
Horizontal=�.+��� $��Center=�$��50��$��导入数量:��$��5q��� $��Center=�$
��Left=�$��50��$��5q����� *Paper.ImportNum+�� $��Center=�$��50��$��实际题量:��$��5q��� $��Center=�$
��Left=�$��50��$��5q����� *Paper.ImportValidNum+�� $��Center=�$��50��$��最大索引:��$��5q��� $��Center=�$
��Left=�$��50��$��5q����� *Paper.MaxIndex+�� $��Center=�$��50��$��错误数量:��$��5q��� $��Center=�$
��Left=�$��50��$��5q����� *Paper.WrongNum+�� $��Center=�$��50��$��错误实际题数:��$��5q��� $��Center=�$
��Left=�$��50��$��5q����� *Paper.WrongKind+