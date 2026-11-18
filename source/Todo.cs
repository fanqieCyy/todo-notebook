using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Drawing.Drawing2D;

public static class Branding {
 public static readonly Icon AppIcon=new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("app.ico"));
}

public static class Paper {
 public static readonly Color Background=ColorTranslator.FromHtml("#F5F4EF"), Surface=ColorTranslator.FromHtml("#FFFEFA"), Sidebar=ColorTranslator.FromHtml("#EEF0E7"), Ink=ColorTranslator.FromHtml("#203D35"), Accent=ColorTranslator.FromHtml("#27644F"), Muted=ColorTranslator.FromHtml("#7A847B"), Border=ColorTranslator.FromHtml("#E1E5DA"), Selected=ColorTranslator.FromHtml("#E3E9D9"), Warning=ColorTranslator.FromHtml("#F3EBD5"), WarningText=ColorTranslator.FromHtml("#725633"), Error=ColorTranslator.FromHtml("#F3E2D9"), ErrorText=ColorTranslator.FromHtml("#853E31");
 public const int Radius=12, ButtonRadius=8, Space=36;
 public static GraphicsPath Shape(Rectangle r,int radius){var p=new GraphicsPath();int d=radius*2;p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
 public static void Card(Graphics g,Rectangle r,Color fill,int radius){g.SmoothingMode=SmoothingMode.AntiAlias;using(var p=Shape(r,radius)){using(var b=new SolidBrush(fill))g.FillPath(b,p);using(var pen=new Pen(Border))g.DrawPath(pen,p);}}
}
public class PaperPanel : Panel {
 public PaperPanel(){DoubleBuffered=true;BackColor=Paper.Surface;Padding=new Padding(12);}
 protected override void OnPaintBackground(PaintEventArgs e){e.Graphics.Clear(Parent==null?Paper.Background:Parent.BackColor);Paper.Card(e.Graphics,new Rectangle(0,0,Width-1,Height-1),BackColor,Paper.Radius);}
}
public class PaperTable : TableLayoutPanel {
 public PaperTable(){DoubleBuffered=true;BackColor=Paper.Surface;}
 protected override void OnPaintBackground(PaintEventArgs e){e.Graphics.Clear(Parent==null?Paper.Background:Parent.BackColor);Paper.Card(e.Graphics,new Rectangle(0,0,Width-1,Height-1),BackColor,Paper.Radius);}
}
public static class PaperMessage {
 public static DialogResult Show(string message,string title){return Show(message,title,MessageBoxButtons.OK);}
 public static DialogResult Show(string message,string title,MessageBoxButtons buttons){
  using(var form=new Form {Icon=Branding.AppIcon,Text=title,Size=new Size(500,310),MinimumSize=new Size(460,290),StartPosition=FormStartPosition.CenterParent,BackColor=Paper.Surface,ForeColor=Paper.Ink,Font=new Font("Microsoft YaHei UI",10),MaximizeBox=false,MinimizeBox=false}){
   var root=new TableLayoutPanel {Dock=DockStyle.Fill,Padding=new Padding(24),RowCount=3,ColumnCount=1};root.RowStyles.Add(new RowStyle(SizeType.Absolute,42));root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,44));form.Controls.Add(root);
   bool error=title.Contains("失败")||title.Contains("未保存");
   root.Controls.Add(new Label {Text=title,AutoSize=true,Font=new Font(form.Font.FontFamily,17),ForeColor=error?Paper.ErrorText:Paper.Ink});
   root.Controls.Add(new TextBox {Text=message,ReadOnly=true,Multiline=true,ScrollBars=ScrollBars.Vertical,Dock=DockStyle.Fill,BorderStyle=BorderStyle.None,BackColor=error?Paper.Error:Paper.Surface,ForeColor=error?Paper.ErrorText:Paper.Ink});
   var actions=new FlowLayoutPanel {Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(0,6,0,0)};root.Controls.Add(actions);
   var ok=new SoftButton {Text=buttons==MessageBoxButtons.YesNo?"确认删除":"知道了",Primary=true,Size=new Size(104,36),DialogResult=buttons==MessageBoxButtons.YesNo?DialogResult.Yes:DialogResult.OK};actions.Controls.Add(ok);form.AcceptButton=ok;
   if(buttons==MessageBoxButtons.YesNo){var cancel=new SoftButton {Text="取消",Size=new Size(80,36),BackColor=Paper.Sidebar,ForeColor=Paper.Ink,DialogResult=DialogResult.No};actions.Controls.Add(cancel);form.CancelButton=cancel;}else form.CancelButton=ok;
   return form.ShowDialog(Form.ActiveForm);
  }
 }
}
public class SoftButton : Button {
 public bool Primary, SelectedStyle;
 bool hover;
 float fade; Timer animation;
 public SoftButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);Cursor=Cursors.Hand;animation=new Timer {Interval=16};animation.Tick+=(s,e)=>{fade=Math.Max(0,Math.Min(1,fade+(hover?0.1f:-0.1f)));Invalidate();if(fade==0||fade==1)animation.Stop();};}
 protected override void Dispose(bool disposing){if(disposing)animation.Dispose();base.Dispose(disposing);}
 protected override void OnMouseEnter(EventArgs e){hover=true;animation.Start();base.OnMouseEnter(e);}
 protected override void OnMouseLeave(EventArgs e){hover=false;animation.Start();base.OnMouseLeave(e);}
 protected override void OnPaint(PaintEventArgs e){
  e.Graphics.Clear(Parent==null?BackColor:Parent.BackColor); e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;var r=new Rectangle(1,1,Width-3,Height-3);
  using(var p=Paper.Shape(r,Paper.ButtonRadius)){
   Color start=Primary?Paper.Accent:SelectedStyle?Paper.Selected:BackColor, end=Primary?Paper.Ink:Paper.Selected;
   Color fill=!Enabled?Paper.Border:Color.FromArgb((int)(start.R+(end.R-start.R)*fade),(int)(start.G+(end.G-start.G)*fade),(int)(start.B+(end.B-start.B)*fade));
   using(var b=new SolidBrush(fill))e.Graphics.FillPath(b,p);
   if(Focused)using(var pen=new Pen(Paper.Accent,2))e.Graphics.DrawPath(pen,p);
  }TextRenderer.DrawText(e.Graphics,Text,Font,r,!Enabled?Paper.Muted:Primary?Paper.Surface:ForeColor,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);
 }
}

public class TaskItem {
 public string Id = Guid.NewGuid().ToString();
 public string Title;
 public DateTime Created = DateTime.Now;
 public DateTime? Due;
 public bool Done;
 public bool Quiet;
}
public class Store {
 public List<TaskItem> Items = new List<TaskItem>();
 public string PathName;
 public Store(string path) { PathName=path; }
 public void Load() {
  if (!File.Exists(PathName)) return;
  using(var f=File.OpenRead(PathName)) Items=(List<TaskItem>)new XmlSerializer(typeof(List<TaskItem>)).Deserialize(f);
 }
 public void Save() {
  Directory.CreateDirectory(Path.GetDirectoryName(PathName));
  string tmp=PathName+".tmp";
  using(var f=File.Create(tmp)) new XmlSerializer(typeof(List<TaskItem>)).Serialize(f,Items);
  if(File.Exists(PathName)) File.Replace(tmp,PathName,PathName+".bak"); else File.Move(tmp,PathName);
 }
 public IEnumerable<TaskItem> DueNow() { return Items.Where(x=>!x.Done && !x.Quiet && x.Due.HasValue && x.Due.Value<=DateTime.Now); }
}
public class MainForm : Form {
 Store store; TextBox input; DateTimePicker when; CheckBox remind; ListView list;
 ComboBox filter; Label status, pageTitle, summary, empty; NotifyIcon tray; Timer timer; bool exiting;
 SoftButton[] navigation=new SoftButton[4]; FlowLayoutPanel reminderOptions; Label composerHint;
 HashSet<string> alerts=new HashSet<string>();
 Color ink=Paper.Ink, accent=Paper.Accent;
 public MainForm(Store data) {
  store=data; Text="待办小本"; Icon=Branding.AppIcon; Size=new Size(1120,800); MinimumSize=new Size(760,640);
  AutoScaleMode=AutoScaleMode.None; StartPosition=FormStartPosition.CenterScreen;
  Font=new Font("Microsoft YaHei UI",10.5f); BackColor=Paper.Background; ForeColor=ink;
  var shell=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=2,RowCount=1,Margin=Padding.Empty};
  shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,220));shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));Controls.Add(shell);
  var side=new Panel {Dock=DockStyle.Fill,BackColor=Paper.Sidebar,Margin=Padding.Empty};shell.Controls.Add(side,0,0);
  side.Controls.Add(new Label {Text="待办小本",Font=new Font(Font.FontFamily,16,FontStyle.Regular),ForeColor=accent,Location=new Point(24,32),Size=new Size(180,38)});
  side.Controls.Add(new Label {Text="留一点空间，给重要的事",Font=new Font(Font.FontFamily,9),ForeColor=Paper.Muted,Location=new Point(24,81),Size=new Size(180,24)});
  side.Controls.Add(new Label {Text="我的清单",Font=new Font(Font.FontFamily,9),ForeColor=Paper.Muted,Location=new Point(24,145),Size=new Size(170,24)});
  filter=new ComboBox();filter.Items.AddRange(new object[]{"全部待办","今天","已完成","随手记"});filter.SelectedIndex=0;
  for(int i=0;i<4;i++){int index=i;var b=(SoftButton)Button(filter.Items[i].ToString(),(s,e)=>{filter.SelectedIndex=index;});b.AutoSize=false;b.SetBounds(16,180+i*55,174,44);navigation[i]=b;side.Controls.Add(b);}
  filter.SelectedIndexChanged+=(s,e)=>RefreshList();
  var backup=Button("导出备份",Export);backup.AutoSize=false;backup.SetBounds(20,425,166,40);side.Controls.Add(backup);
  var local=new Label {Text="数据仅保存在本机\n\n关闭窗口后，提醒仍在。",ForeColor=Paper.Muted,Font=new Font(Font.FontFamily,9),AutoSize=false,Size=new Size(178,76),Left=24,Anchor=AnchorStyles.Left|AnchorStyles.Bottom};local.Dock=DockStyle.Bottom;local.Padding=new Padding(22,0,0,0);side.Controls.Add(local);
  var root=new TableLayoutPanel {Dock=DockStyle.Fill,Padding=new Padding(Paper.Space,32,Paper.Space,20),RowCount=6,ColumnCount=1,Margin=Padding.Empty};
  foreach(int h in new[]{108,164,60})root.RowStyles.Add(new RowStyle(SizeType.Absolute,h));root.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.RowStyles.Add(new RowStyle(SizeType.Absolute,34));root.RowStyles.Add(new RowStyle(SizeType.Absolute,32));shell.Controls.Add(root,1,0);
  var header=new Panel {Dock=DockStyle.Fill};pageTitle=new Label {Text="全部待办",Font=new Font(Font.FontFamily,23,FontStyle.Regular),AutoSize=true,Location=new Point(0,0)};summary=new Label {AutoSize=false,Size=new Size(650,46),ForeColor=Paper.Muted,Location=new Point(3,58)};header.Controls.Add(pageTitle);header.Controls.Add(summary);root.Controls.Add(header);
  var composer=new PaperTable {Dock=DockStyle.Fill,BackColor=Paper.Surface,Padding=new Padding(16,12,16,12),ColumnCount=1,RowCount=3,Margin=new Padding(0,0,0,8)};composer.RowStyles.Add(new RowStyle(SizeType.Absolute,24));composer.RowStyles.Add(new RowStyle(SizeType.Absolute,46));composer.RowStyles.Add(new RowStyle(SizeType.Percent,100));root.Controls.Add(composer);
  composerHint=new Label {Text="记下一件想做的事",AutoSize=true,ForeColor=Paper.Muted,Font=new Font(Font.FontFamily,9)}; composer.Controls.Add(composerHint);
  var entry=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=2,RowCount=1,Margin=Padding.Empty};entry.RowStyles.Add(new RowStyle(SizeType.Percent,100));entry.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));entry.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,112));
  input=new TextBox {Dock=DockStyle.Fill,Font=new Font(Font.FontFamily,13),BackColor=Paper.Surface,ForeColor=Paper.Ink,BorderStyle=BorderStyle.None,Margin=new Padding(2,7,12,0),AccessibleName="待办内容",MaxLength=500};var inputFrame=new PaperPanel {Dock=DockStyle.Fill,Padding=new Padding(10,8,10,6),Margin=new Padding(0,0,10,0)}; input.Margin=Padding.Empty;inputFrame.Controls.Add(input);entry.Controls.Add(inputFrame); input.Enter+=(s,e)=>inputFrame.BackColor=Paper.Selected;input.Leave+=(s,e)=>inputFrame.BackColor=Paper.Surface;
  var add=(SoftButton)Button("添加待办",Add);add.Primary=true;add.Dock=DockStyle.Fill;entry.Controls.Add(add);composer.Controls.Add(entry);input.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){Add(s,e);e.SuppressKeyPress=true;}};
  var options=reminderOptions=new FlowLayoutPanel {Dock=DockStyle.Fill,WrapContents=true,Margin=Padding.Empty,Padding=new Padding(0,5,0,0)};
  remind=new CheckBox {Text="提醒",AutoSize=true,Padding=new Padding(0,5,0,0),ForeColor=Paper.Muted};
  when=new DateTimePicker {Format=DateTimePickerFormat.Custom,CustomFormat="MM-dd HH:mm",Width=135,Value=DateTime.Now.AddMinutes(10),Enabled=false,Margin=new Padding(3,5,8,0)};remind.CheckedChanged+=(s,e)=>when.Enabled=remind.Checked;options.Controls.Add(remind);options.Controls.Add(when);
  options.Controls.Add(Button("10 分钟后",(s,e)=>SetDue(DateTime.Now.AddMinutes(10))));options.Controls.Add(Button("今晚",(s,e)=>SetDue(DateTime.Today.AddHours(20)>DateTime.Now?DateTime.Today.AddHours(20):DateTime.Today.AddDays(1).AddHours(20))));options.Controls.Add(Button("明天",(s,e)=>SetDue(DateTime.Today.AddDays(1).AddHours(9))));composer.Controls.Add(options);
  var bar=new FlowLayoutPanel {Dock=DockStyle.Fill,Padding=new Padding(0,12,0,0)};bar.Controls.Add(Button("完成 / 恢复",Toggle));bar.Controls.Add(Button("修改",Edit));var delete=Button("删除",Delete);delete.ForeColor=Paper.ErrorText;bar.Controls.Add(delete);bar.Controls.Add(Button("移入随手记",(s,e)=>{var t=Selected();if(t==null||t.Quiet)return;var old=t.Due;t.Quiet=true;t.Due=null;if(!Save()){t.Quiet=false;t.Due=old;}}));root.Controls.Add(bar);
  var listHost=new PaperPanel {Dock=DockStyle.Fill,BackColor=Paper.Surface,Margin=Padding.Empty};root.Controls.Add(listHost);
  list=new ListView {Dock=DockStyle.Fill,View=View.Details,FullRowSelect=true,MultiSelect=false,HideSelection=false,BorderStyle=BorderStyle.None,AccessibleName="待办清单",BackColor=Paper.Surface,ForeColor=Paper.Ink,ShowItemToolTips=true,OwnerDraw=true};list.Columns.Add("待办事项",380);list.Columns.Add("提醒时间",165);list.Columns.Add("状态",95);
  var rowHeight=new ImageList {ImageSize=new Size(1,68)};list.SmallImageList=rowHeight;
  list.DrawColumnHeader+=(s,e)=>{using(var b=new SolidBrush(Paper.Background))e.Graphics.FillRectangle(b,e.Bounds);TextRenderer.DrawText(e.Graphics,e.Header.Text,Font,new Rectangle(e.Bounds.X+16,e.Bounds.Y,e.Bounds.Width-16,e.Bounds.Height),Paper.Muted,TextFormatFlags.VerticalCenter);};
  list.DrawSubItem+=(s,e)=>{var t=(TaskItem)e.Item.Tag;bool selected=e.Item.Selected;using(var b=new SolidBrush(selected?Paper.Selected:Paper.Surface))e.Graphics.FillRectangle(b,e.Bounds);var color=e.ColumnIndex==0?ink:Paper.Muted;if(t.Done)color=Paper.Muted;else if(t.Due.HasValue&&t.Due.Value<=DateTime.Now&&e.ColumnIndex>0)color=Paper.WarningText;var rect=new Rectangle(e.Bounds.X+12,e.Bounds.Y+8,e.Bounds.Width-20,e.Bounds.Height-16);if(e.ColumnIndex==2){var badge=new Rectangle(e.Bounds.X+5,e.Bounds.Y+20,e.Bounds.Width-12,28);using(var shape=Paper.Shape(badge,7))using(var brush=new SolidBrush(t.Done||t.Quiet?Paper.Selected:Paper.Warning))e.Graphics.FillPath(brush,shape);color=t.Done||t.Quiet?Paper.Ink:Paper.WarningText;}TextRenderer.DrawText(e.Graphics,e.SubItem.Text,Font,rect,color,TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis|TextFormatFlags.WordBreak);using(var pen=new Pen(Paper.Border))e.Graphics.DrawLine(pen,e.Bounds.Left,e.Bounds.Bottom-1,e.Bounds.Right,e.Bounds.Bottom-1);};
  list.Resize+=(s,e)=>{list.Columns[1].Width=list.ClientSize.Width<550?125:155;list.Columns[2].Width=88;list.Columns[0].Width=Math.Max(100,list.ClientSize.Width-list.Columns[1].Width-list.Columns[2].Width-4);};list.DoubleClick+=Edit;list.SelectedIndexChanged+=(s,e)=>{foreach(Control control in bar.Controls)control.Enabled=Selected()!=null;};foreach(Control control in bar.Controls)control.Enabled=false;listHost.Controls.Add(list);
  empty=new Label {Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Paper.Muted,Font=new Font(Font.FontFamily,13),BackColor=Paper.Surface};listHost.Controls.Add(empty);
  status=new Label {AutoSize=true,Margin=new Padding(0,10,0,0),Font=new Font(Font.FontFamily,9),ForeColor=Paper.Muted};root.Controls.Add(status);
  var startup=new CheckBox {Text="登录电脑时启动",AutoSize=true,Font=new Font(Font.FontFamily,9),ForeColor=Paper.Muted};
  using(var key=Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))startup.Checked=key!=null&&key.GetValue("TodoNotebook")!=null;
  startup.CheckedChanged+=(s,e)=>{try{using(var key=Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run")){if(startup.Checked)key.SetValue("TodoNotebook","\""+Application.ExecutablePath+"\" --background");else key.DeleteValue("TodoNotebook",false);}}catch(Exception ex){PaperMessage.Show(ex.Message,"启动设置未保存");}};root.Controls.Add(startup);
  tray=new NotifyIcon {Icon=Branding.AppIcon,Text="待办小本",Visible=true};var menu=new ContextMenuStrip();menu.Items.Add("打开待办小本",null,(s,e)=>Reveal());menu.Items.Add("退出程序",null,(s,e)=>{exiting=true;Close();});tray.ContextMenuStrip=menu;tray.DoubleClick+=(s,e)=>Reveal();
  FormClosing+=(s,e)=>{if(!exiting&&e.CloseReason==CloseReason.UserClosing){e.Cancel=true;Hide();tray.ShowBalloonTip(2000,"待办小本","已收起到右下角，仍会提醒。右键托盘图标可退出。",ToolTipIcon.Info);}};
  FormClosed+=(s,e)=>{timer.Stop();tray.Dispose();rowHeight.Dispose();filter.Dispose();};timer=new Timer {Interval=1000};timer.Tick+=(s,e)=>CheckReminders();timer.Start();RefreshList();
  Action layout=()=>{bool compact=ClientSize.Width<940;shell.ColumnStyles[0].Width=compact?136:220;root.Padding=new Padding(compact?20:Paper.Space,compact?22:32,compact?20:Paper.Space,16);foreach(var nav in navigation){nav.Left=12;nav.Width=compact?112:196;}backup.Left=12;backup.Width=compact?112:196;foreach(Control c in side.Controls)if(c is Label && c!=local)c.Visible=!compact;local.Visible=!compact;root.RowStyles[1].Height=compact?188:164;summary.Width=Math.Max(200,header.Width-8);};Resize+=(s,e)=>layout();Shown+=(s,e)=>{layout();if(Environment.GetCommandLineArgs().Contains("--background"))Hide();else input.Focus();};
 }
 Button Button(string text,EventHandler action){var b=new SoftButton {Text=text,AutoSize=false,Width=Math.Max(70,TextRenderer.MeasureText(text,Font).Width+24),Height=38,FlatStyle=FlatStyle.Flat,BackColor=Paper.Sidebar,ForeColor=ink,Margin=new Padding(0,0,8,0)};b.Click+=action;return b;}
 void SetDue(DateTime value){remind.Checked=true;when.Value=value;}
 bool Save(){UseWaitCursor=true;status.Text="正在保存…";try{store.Save();RefreshList();return true;}catch(Exception ex){PaperMessage.Show("无法保存，请检查文件夹权限或磁盘空间。\n"+ex.Message,"保存失败");return false;}finally{UseWaitCursor=false;}}
 void Add(object s,EventArgs e){if(string.IsNullOrWhiteSpace(input.Text)){status.Text="先写下要做的事，再添加。";status.ForeColor=Paper.ErrorText;input.Focus();return;}status.ForeColor=Paper.Muted;var t=new TaskItem {Title=input.Text.Trim(),Quiet=filter.SelectedIndex==3,Due=filter.SelectedIndex!=3&&remind.Checked?(DateTime?)when.Value:null};store.Items.Add(t);if(Save()){input.Clear();remind.Checked=false;input.Focus();}else store.Items.Remove(t);}
 TaskItem Selected(){return list.SelectedItems.Count==0?null:(TaskItem)list.SelectedItems[0].Tag;}
 void Toggle(object s,EventArgs e){var t=Selected();if(t==null)return;t.Done=!t.Done;if(!Save())t.Done=!t.Done;}
 void Delete(object s,EventArgs e){var t=Selected();if(t==null)return;if(PaperMessage.Show("删除“"+t.Title+"”？","删除待办",MessageBoxButtons.YesNo)==DialogResult.Yes){store.Items.Remove(t);if(!Save())store.Items.Add(t);}}
 void Edit(object s,EventArgs e){var t=Selected();if(t==null)return;
  using(var f=new Form {Icon=Branding.AppIcon,Text="修改待办",Size=new Size(540,365),StartPosition=FormStartPosition.CenterParent,Font=Font,BackColor=Paper.Surface,ForeColor=Paper.Ink,FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false}){
   var title=new Label {Text="修改待办",Font=new Font(Font.FontFamily,20),Left=28,Top=20,Width=460,Height=40};
   var frame=new PaperPanel {Left=28,Top=76,Width=466,Height=110,Padding=new Padding(12)};
   var text=new TextBox {Text=t.Title,Multiline=true,ScrollBars=ScrollBars.Vertical,Dock=DockStyle.Fill,BorderStyle=BorderStyle.None,BackColor=Paper.Surface,ForeColor=Paper.Ink,MaxLength=500};frame.Controls.Add(text);
   var check=new CheckBox {Text=t.Quiet?"随手记 · 不提醒":"提醒",Checked=!t.Quiet&&t.Due.HasValue,Enabled=!t.Quiet,Left=28,Top=205,Width=t.Quiet?200:70,Height=30};
   var date=new DateTimePicker {Left=110,Top=205,Width=260,Format=DateTimePickerFormat.Custom,CustomFormat="yyyy-MM-dd HH:mm",Value=t.Due??DateTime.Now.AddMinutes(10),Enabled=check.Checked,Visible=!t.Quiet};check.CheckedChanged+=(a,b)=>date.Enabled=check.Checked;
   var ok=(SoftButton)Button("保存修改",(a,b)=>{if(!string.IsNullOrWhiteSpace(text.Text))f.DialogResult=DialogResult.OK;else {frame.BackColor=Paper.Error;text.Focus();}});ok.Primary=true;ok.SetBounds(374,264,120,40);
   var cancel=Button("取消",(a,b)=>f.DialogResult=DialogResult.Cancel);cancel.SetBounds(280,264,80,40);f.CancelButton=cancel;f.Controls.AddRange(new Control[]{title,frame,check,date,cancel,ok});
   if(f.ShowDialog(this)==DialogResult.OK){string old=t.Title;var due=t.Due;t.Title=text.Text.Trim();t.Due=!t.Quiet&&check.Checked?(DateTime?)date.Value:null;if(!Save()){t.Title=old;t.Due=due;}}
  }
 }
 void Export(object s,EventArgs e){using(var dialog=new SaveFileDialog {Filter="待办备份 (*.xml)|*.xml",FileName="待办备份-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".xml"}){if(dialog.ShowDialog()==DialogResult.OK){try{using(var f=File.Create(dialog.FileName))new XmlSerializer(typeof(List<TaskItem>)).Serialize(f,store.Items);PaperMessage.Show("备份已导出。","待办小本");}catch(Exception ex){PaperMessage.Show(ex.Message,"导出失败");}}}}
 void RefreshList(){if(list==null)return;list.BeginUpdate();list.Items.Clear();var items=store.Items.Where(t=>filter.SelectedIndex==2?t.Done:filter.SelectedIndex==3?(!t.Done&&t.Quiet):(!t.Done&&!t.Quiet));if(filter.SelectedIndex==1)items=items.Where(t=>t.Due.HasValue?t.Due.Value.Date<=DateTime.Today:t.Created.Date==DateTime.Today);foreach(var t in items.OrderBy(t=>t.Due??DateTime.MaxValue).ThenByDescending(t=>t.Created)){bool late=!t.Done&&t.Due.HasValue&&t.Due.Value<=DateTime.Now;var row=new ListViewItem(new[]{t.Title,t.Due.HasValue?t.Due.Value.ToString("MM-dd HH:mm"):"不提醒",t.Done?"已完成":t.Quiet?"随手记":late?"已到期":"待办"}) {Tag=t,ToolTipText=t.Title};if(late)row.ForeColor=Paper.WarningText;list.Items.Add(row);}list.EndUpdate(); status.Text="双击事项可修改  ·  数据保存在本机"; pageTitle.Text=filter.SelectedItem.ToString(); reminderOptions.Visible=filter.SelectedIndex!=3;composerHint.Text=filter.SelectedIndex==3?"只记下来，不催促，也不弹提醒":"记下一件想做的事"; summary.Text=(filter.SelectedIndex==3?"为有空时的自己，收好每一个念头。":filter.SelectedIndex==2?"每一件完成的小事，都在这里留下记录。":filter.SelectedIndex==1?"把注意力留给今天，慢慢完成重要的事。":"把想做的事安放在这里，一件一件来。")+"\n"+store.Items.Count(t=>!t.Done)+" 项待办 · "+store.Items.Count(t=>t.Done)+" 项已完成"; for(int i=0;i<4;i++){navigation[i].SelectedStyle=i==filter.SelectedIndex;navigation[i].Invalidate();} empty.Text=filter.SelectedIndex==3?"留给有空时的自己\n\n记下想做的事，看到时再慢慢完成。":filter.SelectedIndex==2?"还没有已完成的事项\n\n每完成一件，都值得一个小小的肯定。":filter.SelectedIndex==1?"今天的清单很轻盈\n\n在上方记下今天想做的事吧。":"从一件小事开始\n\n在上方输入待办，按 Enter 就能记下来。";empty.Visible=list.Items.Count==0;if(list.Items.Count==0)empty.BringToFront();}
 public void Preview(string[] args){if(args.Contains("--quiet"))filter.SelectedIndex=3;if(args.Contains("--done"))filter.SelectedIndex=2;if(args.Contains("--today"))filter.SelectedIndex=1;if(args.Contains("--empty")){store.Items.Clear();RefreshList();}if(args.Contains("--reminder"))ShowReminder(store.Items[0]);if(args.Contains("--edit")){list.Items[0].Selected=true;BeginInvoke(new Action(()=>Edit(this,EventArgs.Empty)));}}
 void Reveal(){Show();WindowState=FormWindowState.Normal;Activate();RefreshList();}
 void CheckReminders(){foreach(var t in store.DueNow().ToList()){if(alerts.Add(t.Id))ShowReminder(t);} }
 void ShowReminder(TaskItem t){var f=new Form {Icon=Branding.AppIcon,Text="待办提醒",Size=new Size(480,340),FormBorderStyle=FormBorderStyle.FixedDialog,StartPosition=FormStartPosition.CenterScreen,TopMost=true,BackColor=Paper.Surface,ForeColor=Paper.Ink,Font=Font,MaximizeBox=false,MinimizeBox=false};var heading=new Label {Text="到时间了，看看这件事",Left=28,Top=24,Width=420,Height=40,Font=new Font(Font.FontFamily,18),ForeColor=Paper.Ink};var label=new TextBox {Text=t.Title,ReadOnly=true,Multiline=true,ScrollBars=ScrollBars.Vertical,BorderStyle=BorderStyle.None,BackColor=Paper.Surface,ForeColor=Paper.Ink,Left=28,Top=82,Width=404,Height=120,Font=new Font(Font.FontFamily,12)};bool handled=false;
  var done=Button("完成",(s,e)=>{t.Done=true;if(Save()){handled=true;f.Close();}else t.Done=false;});done.SetBounds(28,238,100,40);((SoftButton)done).Primary=true;
  var later=Button("10 分钟后提醒",(s,e)=>{var old=t.Due;t.Due=DateTime.Now.AddMinutes(10);if(Save()){handled=true;f.Close();}else t.Due=old;});later.SetBounds(142,238,170,40);
  f.Controls.AddRange(new Control[]{heading,label,done,later}); f.FormClosing+=(s,e)=>{if(!handled && e.CloseReason==CloseReason.UserClosing){var old=t.Due;t.Due=DateTime.Now.AddMinutes(10);if(!Save()){t.Due=old;e.Cancel=true;}}};f.FormClosed+=(s,e)=>alerts.Remove(t.Id);f.Show();System.Media.SystemSounds.Asterisk.Play();RefreshList();}
}
public static class Program {
 [STAThread] public static int Main(string[] args){
  Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
  if(args.Contains("--self-test")){string p=Path.Combine(Path.GetTempPath(),"todo-test-"+Guid.NewGuid()+".xml");try{var s=new Store(p);s.Items.Add(new TaskItem{Title="中文存储测试",Due=DateTime.Now.AddMinutes(-1)});s.Save();var b=new Store(p);b.Load();if(b.Items[0].Title!="中文存储测试"||b.DueNow().Count()!=1)return 1;b.Items[0].Due=DateTime.Now.AddMinutes(10);b.Save();if(b.DueNow().Any()||!File.Exists(p+".bak"))return 2;b.Items[0].Due=DateTime.Now.AddMinutes(-1);b.Items[0].Done=true;if(b.DueNow().Any())return 3;b.Items.Add(new TaskItem{Title="随手记不提醒",Quiet=true,Due=DateTime.Now.AddMinutes(-5)});b.Save();var c=new Store(p);c.Load();if(!c.Items[1].Quiet||c.DueNow().Any())return 4;return 0;}finally{foreach(var f in new[]{p,p+".bak",p+".tmp"})if(File.Exists(f))File.Delete(f);}}
  if(args.Contains("--preview")){var sample=new Store(Path.Combine(Path.GetTempPath(),"TodoNotebook-preview.xml"));sample.Items.Add(new TaskItem{Title="整理书桌，让常用的东西各归其位",Due=DateTime.Now.AddDays(2)});sample.Items.Add(new TaskItem{Title="读完收藏的文章，记录值得留下的想法和问题",Due=null});sample.Items.Add(new TaskItem{Title="周末去公园散步，看看这个季节的树",Quiet=true});sample.Items.Add(new TaskItem{Title="整理本周的笔记",Done=true});var form=new MainForm(sample);if(args.Contains("--small"))form.Size=new Size(760,640);form.Shown+=(s,e)=>form.Preview(args);Application.Run(form);return 0;}
  bool first;using(var mutex=new System.Threading.Mutex(true,"Local\\TodoNotebook.Desktop",out first)){if(!first){PaperMessage.Show("待办小本已在运行，请双击右下角托盘图标打开。","待办小本");return 0;}var store=new Store(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"data","tasks.xml"));try{store.Load();}catch(Exception ex){PaperMessage.Show("待办文件读取失败，已停止启动，避免覆盖原数据。\n可使用 data 文件夹中的 tasks.xml.bak 恢复。\n"+ex.Message,"待办小本");return 1;}Application.Run(new MainForm(store));}return 0;
 }
}









