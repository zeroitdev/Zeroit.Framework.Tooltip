// ***********************************************************************
// Assembly         : Zeroit.Framework.Tooltip
// Author           : ZEROIT
// Created          : 11-22-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-21-2018
// ***********************************************************************
// <copyright file="TetraTooltip.cs" company="Zeroit Dev Technologies">
//    This program is for creating Tooltip controls.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;

namespace Zeroit.Framework.Tooltip
{

    /// <summary>
    /// Custom tooltip.
    /// </summary>
    /// <seealso cref="System.ComponentModel.Component" />
    /// <seealso cref="System.ComponentModel.IExtenderProvider" />
    [ProvideProperty("ZeroitTetraToolTip", typeof(System.Windows.Forms.Control)), ProvideProperty("ToolTipTitle", typeof(System.Windows.Forms.Control)), ProvideProperty("ToolTipImage", typeof(System.Windows.Forms.Control))]
    public class ZeroitTetraToolTip : System.ComponentModel.Component, IExtenderProvider
    {
        #region Public Events
        /// <summary>
        /// Event before tooltip is displayed.  This event is raised when OwnerDraw property is set to true.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PopupEventArgs"/> instance containing the event data.</param>
        public delegate void PopupEventHandler(object sender, PopupEventArgs e);
        /// <summary>
        /// Occurs when [popup].
        /// </summary>
        public event PopupEventHandler Popup;
        /// <summary>
        /// Event when the tooltip surface is drawn.  This event is raised when OwnerDraw property is set to true.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DrawEventArgs"/> instance containing the event data.</param>
        public delegate void DrawEventHandler(object sender, DrawEventArgs e);
        /// <summary>
        /// Occurs when [draw].
        /// </summary>
        public event DrawEventHandler Draw;
        /// <summary>
        /// Event when the tooltip background is drawn.  This event is raised when OwnerDrawBackground property is set to true.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DrawEventArgs"/> instance containing the event data.</param>
        public delegate void DrawBackgroundEventHandler(object sender, DrawEventArgs e);
        /// <summary>
        /// Occurs when [draw background].
        /// </summary>
        public event DrawBackgroundEventHandler DrawBackground;
        #endregion

        #region Declarations
        /// <summary>
        /// The parent
        /// </summary>
        private System.Windows.Forms.Control _parent = null; // Owner of this tooltip
        /// <summary>
        /// The control
        /// </summary>
        private System.Windows.Forms.Control _control = null;
        /// <summary>
        /// The animation speed
        /// </summary>
        private int _animationSpeed = 20; // In milliseconds, interval to fade - in / out
        /// <summary>
        /// The show shadow
        /// </summary>
        private bool _showShadow = true;
        /// <summary>
        /// The form
        /// </summary>
        private TooltipForm _form; // Form to display the tooltip
        /// <summary>
        /// The automatic close
        /// </summary>
        private int _autoClose = 3000; // In milliseconds, tooltip will automatically closed if this period passed
        /// <summary>
        /// The enable automatic close
        /// </summary>
        private bool _enableAutoClose = true;
        /// <summary>
        /// The owner draw
        /// </summary>
        private bool _ownerDraw = false;
        /// <summary>
        /// The owner draw background
        /// </summary>
        private bool _ownerDrawBackground = false;
        /// <summary>
        /// The location
        /// </summary>
        private ToolTipLocation _location = ToolTipLocation.Auto;
        /// <summary>
        /// The custom location
        /// </summary>
        private Point _customLocation = new Point(0, 0);
        // Support for IExtenderProvider
        /// <summary>
        /// The texts
        /// </summary>
        private Hashtable _texts;
        /// <summary>
        /// The titles
        /// </summary>
        private Hashtable _titles;
        /// <summary>
        /// The images
        /// </summary>
        private Hashtable _images;
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor of the tooltip with an owner control specified.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ZeroitTetraToolTip(System.Windows.Forms.Control parent)
        {
            _parent = parent;
            _texts = new Hashtable();
            _titles = new Hashtable();
            _images = new Hashtable();
            _ownerDraw = true;
        }
        /// <summary>
        /// ZeroitTetraToolTip constructor.
        /// </summary>
        public ZeroitTetraToolTip()
        {
            _texts = new Hashtable();
            _titles = new Hashtable();
            _images = new Hashtable();
            _ownerDraw = false;
        }
        /// <summary>
        /// Show ZeroitTetraToolTip with specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        public void show(System.Windows.Forms.Control control)
        {
            TooltipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null)
            {
                _form.invokeClose();
            }
            Size tooltipSize = new Size();
            if (_ownerDraw || _ownerDrawBackground)
            {
                PopupEventArgs e = null;
                e = new PopupEventArgs();
                if (Popup != null)
                    Popup(this, e);
                tooltipSize = e.Size;
            }
            else
            {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Zeroit.Framework.Tooltip.ControlRenderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new TooltipForm(this, tooltipSize);
        }
        /// <summary>
        /// Show ZeroitTetraToolTip with specified control and location.  The ZeroitTetraToolTip location is relative to the control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="location">The location.</param>
        public void show(System.Windows.Forms.Control control, Point location)
        {
            TooltipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null)
            {
                _form.invokeClose();
            }
            Size tooltipSize = new Size();
            if (_ownerDraw || _ownerDrawBackground)
            {
                PopupEventArgs e = null;
                e = new PopupEventArgs();
                if (Popup != null)
                    Popup(this, e);
                tooltipSize = e.Size;
            }
            else
            {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Zeroit.Framework.Tooltip.ControlRenderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new TooltipForm(this, tooltipSize, location);
        }
        /// <summary>
        /// Show ZeroitTetraToolTip with specified control and rectangle area.  This area is where the tooltip must avoid to cover.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="rect">The rect.</param>
        public void show(System.Windows.Forms.Control control, Rectangle rect)
        {
            TooltipForm._showShadow = _showShadow;
            _control = control;
            if (_form != null)
            {
                _form.invokeClose();
            }
            Size tooltipSize = new Size();
            if (_ownerDraw || _ownerDrawBackground)
            {
                PopupEventArgs e = null;
                e = new PopupEventArgs();
                if (Popup != null)
                    Popup(this, e);
                tooltipSize = e.Size;
            }
            else
            {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                tooltipSize = Zeroit.Framework.Tooltip.ControlRenderer.ToolTip.measureSize(tTitle, tText, tImage);
            }
            _form = new TooltipForm(this, tooltipSize, rect);
        }
        /// <summary>
        /// Hide the ZeroitTetraToolTip.
        /// </summary>
        public void hide()
        {
            try
            {
                _form.DoClose();
            }
            catch (Exception ex)
            {
            }
        }
        // Extended property for ZeroitTetraToolTip property.
        /// <summary>
        /// Gets the tool tip.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        [EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor)), DefaultValue("")]
        public string GetToolTip(object obj)
        {
            string tText = Convert.ToString(_texts[obj]);
            if (tText == null)
            {
                tText = string.Empty;
            }
            return tText;
        }
        /// <summary>
        /// Sets the tool tip.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        [EditorAttribute(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public void SetToolTip(object obj, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            if (value.Length == 0)
            {
                _texts.Remove(obj);
            }
            else
            {
                _texts[obj] = value;
            }
            if (hasToolTip((System.Windows.Forms.Control)obj))
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter += ctrlMouseEnter;
                    ctrl.MouseLeave += ctrlMouseLeave;
                    ctrl.MouseDown += ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter += tsiMouseEnter;
                    anItem.MouseLeave += ctrlMouseLeave;
                    anItem.MouseDown += ctrlMouseDown;
                }
            }
            else
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter -= ctrlMouseEnter;
                    ctrl.MouseLeave -= ctrlMouseLeave;
                    ctrl.MouseDown -= ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter -= tsiMouseEnter;
                    anItem.MouseLeave -= ctrlMouseLeave;
                    anItem.MouseDown -= ctrlMouseDown;
                }
            }
        }
        /// <summary>
        /// Gets the tool tip title.
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <returns>System.String.</returns>
        [DefaultValue("")]
        public string GetToolTipTitle(System.Windows.Forms.Control ctrl)
        {
            string tTitle = Convert.ToString(_titles[ctrl]);
            if (tTitle == null)
            {
                tTitle = string.Empty;
            }
            return tTitle;
        }
        /// <summary>
        /// Sets the tool tip title.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetToolTipTitle(object obj, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            if (value.Length == 0)
            {
                _titles.Remove(obj);
            }
            else
            {
                _titles[obj] = value;
            }
            if (hasToolTip((System.Windows.Forms.Control)obj))
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter += ctrlMouseEnter;
                    ctrl.MouseLeave += ctrlMouseLeave;
                    ctrl.MouseDown += ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter += tsiMouseEnter;
                    anItem.MouseLeave += ctrlMouseLeave;
                    anItem.MouseDown += ctrlMouseDown;
                }
            }
            else
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter -= ctrlMouseEnter;
                    ctrl.MouseLeave -= ctrlMouseLeave;
                    ctrl.MouseDown -= ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter -= tsiMouseEnter;
                    anItem.MouseLeave -= ctrlMouseLeave;
                    anItem.MouseDown -= ctrlMouseDown;
                }
            }
        }
        /// <summary>
        /// Gets the tool tip image.
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <returns>Image.</returns>
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.UITypeEditor)), DefaultValue(typeof(Image), "Nothing")]
        public Image GetToolTipImage(System.Windows.Forms.Control ctrl)
        {
            return (Image)_images[ctrl];
        }
        /// <summary>
        /// Sets the tool tip image.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public void SetToolTipImage(object obj, Image value)
        {
            if (value == null)
            {
                _images.Remove(obj);
            }
            else
            {
                _images[obj] = value;
            }
            if (hasToolTip((System.Windows.Forms.Control)obj))
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter += ctrlMouseEnter;
                    ctrl.MouseLeave += ctrlMouseLeave;
                    ctrl.MouseDown += ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter += tsiMouseEnter;
                    anItem.MouseLeave += ctrlMouseLeave;
                    anItem.MouseDown += ctrlMouseDown;
                }
            }
            else
            {
                if ((obj) is System.Windows.Forms.Control)
                {
                    System.Windows.Forms.Control ctrl = (System.Windows.Forms.Control)obj;
                    ctrl.MouseEnter -= ctrlMouseEnter;
                    ctrl.MouseLeave -= ctrlMouseLeave;
                    ctrl.MouseDown -= ctrlMouseDown;
                }
                else if ((obj) is ToolStripItem)
                {
                    ToolStripItem anItem = (ToolStripItem)obj;
                    anItem.MouseEnter -= tsiMouseEnter;
                    anItem.MouseLeave -= ctrlMouseLeave;
                    anItem.MouseDown -= ctrlMouseDown;
                }
            }
        }
        /// <summary>
        /// Specifies whether this object can provide its extender properties to the specified object.
        /// </summary>
        /// <param name="extendee">The <see cref="T:System.Object" /> to receive the extender properties.</param>
        /// <returns>true if this object can provide extender properties to the specified object; otherwise, false.</returns>
        public bool CanExtend(object extendee)
        {
            if ((extendee) is System.Windows.Forms.Control)
            {
                if ((extendee) is System.Windows.Forms.Form)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if ((extendee) is ToolStripItem)
            {
                return true;
            }
            return false;
        }
        // Disposing components
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clear all resources
                _texts.Clear();
                _titles.Clear();
                _images.Clear();
                if (_form != null)
                {
                    _form.DoClose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        // This class, API calls, I got this from an article in CodeProject about layered window (but I forgot)
        #region Private Class
        #region Windows API
        /// <summary>
        /// Struct BLENDFUNCTION
        /// </summary>
        private struct BLENDFUNCTION
        {
            /// <summary>
            /// The blend op
            /// </summary>
            public byte BlendOp;
            /// <summary>
            /// The blend flags
            /// </summary>
            public byte BlendFlags;
            /// <summary>
            /// The source constant alpha
            /// </summary>
            public byte SourceConstantAlpha;
            /// <summary>
            /// The alpha format
            /// </summary>
            public byte AlphaFormat;
        }
        /// <summary>
        /// Updates the layered window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hdcDst">The HDC DST.</param>
        /// <param name="pptDst">The PPT DST.</param>
        /// <param name="psize">The psize.</param>
        /// <param name="hdcSrc">The HDC source.</param>
        /// <param name="pptSrc">The PPT source.</param>
        /// <param name="crKey">The cr key.</param>
        /// <param name="pBlend">The p blend.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, int crKey, ref BLENDFUNCTION pBlend, int dwFlags);
        /// <summary>
        /// Gets the dc.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <returns>IntPtr.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetDC", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);
        /// <summary>
        /// Releases the dc.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hDC">The h dc.</param>
        /// <returns>System.Int32.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ReleaseDC", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        /// <summary>
        /// Creates the compatible dc.
        /// </summary>
        /// <param name="hDC">The h dc.</param>
        /// <returns>IntPtr.</returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        /// <summary>
        /// Deletes the dc.
        /// </summary>
        /// <param name="hDC">The h dc.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteDC", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hDC);
        /// <summary>
        /// Selects the object.
        /// </summary>
        /// <param name="hDC">The h dc.</param>
        /// <param name="hObject">The h object.</param>
        /// <returns>IntPtr.</returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "SelectObject", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="hObject">The h object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nCmdShow">The n command show.</param>
        /// <returns>System.Int32.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ShowWindow", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="wMsg">The w MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns>System.Int32.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        /// <summary>
        /// Releases the capture.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ReleaseCapture", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern void ReleaseCapture();
        /// <summary>
        /// The ws ex layered
        /// </summary>
        private const int WS_EX_LAYERED = 0x80000;
        /// <summary>
        /// The ulw alpha
        /// </summary>
        private const int ULW_ALPHA = 0x2;
        /// <summary>
        /// The ac source over
        /// </summary>
        private const int AC_SRC_OVER = 0x0;
        /// <summary>
        /// The ac source alpha
        /// </summary>
        private const int AC_SRC_ALPHA = 0x1;
        /// <summary>
        /// The ws ex transparent
        /// </summary>
        private const long WS_EX_TRANSPARENT = 0x20L;
        #endregion
        /// <summary>
        /// Class TooltipForm.
        /// </summary>
        /// <seealso cref="System.Windows.Forms.Form" />
        private class TooltipForm : System.Windows.Forms.Form
        {
            /// <summary>
            /// The show shadow
            /// </summary>
            public static bool _showShadow;
            /// <summary>
            /// The closing
            /// </summary>
            private bool _closing = false;
            /// <summary>
            /// The border margin
            /// </summary>
            private const int BORDER_MARGIN = 1;
            /// <summary>
            /// The rect
            /// </summary>
            private Rectangle _rect;
            /// <summary>
            /// The path
            /// </summary>
            private GraphicsPath _path;
            /// <summary>
            /// The bg bitmap
            /// </summary>
            private Bitmap bgBitmap;
            /// <summary>
            /// The t bitmap
            /// </summary>
            private Bitmap tBitmap;
            /// <summary>
            /// The timer
            /// </summary>
            private Timer _timer;
            /// <summary>
            /// The TMR close
            /// </summary>
            private Timer _tmrClose;
            /// <summary>
            /// The m normal position
            /// </summary>
            private Point mNormalPos;
            /// <summary>
            /// The m current bounds
            /// </summary>
            private Rectangle mCurrentBounds;
            /// <summary>
            /// The m popup
            /// </summary>
            private ZeroitTetraToolTip mPopup;
            /// <summary>
            /// The m timer started
            /// </summary>
            private DateTime mTimerStarted;
            /// <summary>
            /// The m progress
            /// </summary>
            private double mProgress;
            /// <summary>
            /// The cs dropshadow
            /// </summary>
            private const int CS_DROPSHADOW = 0x20000;
            /// <summary>
            /// The sw noactivate
            /// </summary>
            private const int SW_NOACTIVATE = 4;
            /// <summary>
            /// The ws ex toolwindow
            /// </summary>
            private const long WS_EX_TOOLWINDOW = 0x80L;
            /// <summary>
            /// The SWP nosize
            /// </summary>
            private const int SWP_NOSIZE = 0x1;
            /// <summary>
            /// The SWP nomove
            /// </summary>
            private const int SWP_NOMOVE = 0x2;
            /// <summary>
            /// The SWP noactivate
            /// </summary>
            private const int SWP_NOACTIVATE = 0x10;
            /// <summary>
            /// The ws popup
            /// </summary>
            private const int WS_POPUP = unchecked((int)0x80000000);
            /// <summary>
            /// The HWND topmost
            /// </summary>
            private IntPtr HWND_TOPMOST = new IntPtr(-1);
            /// <summary>
            /// The mx
            /// </summary>
            private int mx;
            /// <summary>
            /// My
            /// </summary>
            private int _my;
            /// <summary>
            /// The alpha
            /// </summary>
            private int _alpha = 100;
            /// <summary>
            /// The m background image
            /// </summary>
            private static Image mBackgroundImage;
            /// <summary>
            /// Sets the window position.
            /// </summary>
            /// <param name="hWnd">The h WND.</param>
            /// <param name="hWndInsertAfter">The h WND insert after.</param>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="cx">The cx.</param>
            /// <param name="cy">The cy.</param>
            /// <param name="flags">The flags.</param>
            /// <returns>System.Int32.</returns>
            [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
            private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
            /// <summary>
            /// Initializes a new instance of the <see cref="TooltipForm"/> class.
            /// </summary>
            /// <param name="popup">The popup.</param>
            /// <param name="size">The size.</param>
            public TooltipForm(ZeroitTetraToolTip popup, System.Drawing.Size size)
            {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._parent != null)
                {
                    Form parentForm = mPopup._parent.FindForm();
                    if (parentForm != null)
                    {
                        parentForm.AddOwnedForm(this);
                    }
                }
                else
                {
                    if (mPopup._control != null)
                    {
                        Form parentForm = mPopup._control.FindForm();
                        if (parentForm != null)
                        {
                            parentForm.AddOwnedForm(this);
                        }
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow)
                {
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                }
                else
                {
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate();
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = mPopup._autoClose;
                _tmrClose.Tick += AutoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0)
                {
                    _alpha = 0;
                    // I always aim 25 images per seconds.. seems to be a good value
                    // it looks smooth enough on fast computers and do not drain slower one
                    _timer.Interval = mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += Showing;
                    _timer.Start();
                    Showing(null, null);
                }
                else
                {
                    setBitmap(bgBitmap);
                }
                //If mPopup.mDialog Then
                //    ShowDialog()
                //Else
                //    Show()
                //End If
                ShowWindow(this.Handle, SW_NOACTIVATE);
                SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                if (mPopup._enableAutoClose)
                {
                    _tmrClose.Start();
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="TooltipForm"/> class.
            /// </summary>
            /// <param name="popup">The popup.</param>
            /// <param name="size">The size.</param>
            /// <param name="location">The location.</param>
            public TooltipForm(ZeroitTetraToolTip popup, System.Drawing.Size size, Point location)
            {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._parent != null)
                {
                    Form parentForm = mPopup._parent.FindForm();
                    if (parentForm != null)
                    {
                        parentForm.AddOwnedForm(this);
                    }
                }
                else
                {
                    if (mPopup._control != null)
                    {
                        Form parentForm = mPopup._control.FindForm();
                        if (parentForm != null)
                        {
                            parentForm.AddOwnedForm(this);
                        }
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow)
                {
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                }
                else
                {
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate(location);
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = mPopup._autoClose;
                _tmrClose.Tick += AutoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0)
                {
                    _alpha = 0;
                    // I always aim 25 images per seconds.. seems to be a good value
                    // it looks smooth enough on fast computers and do not drain slower one
                    _timer.Interval = mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += Showing;
                    _timer.Start();
                    Showing(null, null);
                }
                else
                {
                    setBitmap(bgBitmap);
                }
                //If mPopup.mDialog Then
                //    ShowDialog()
                //Else
                //    Show()
                //End If
                ShowWindow(this.Handle, SW_NOACTIVATE);
                SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                if (mPopup._enableAutoClose)
                {
                    _tmrClose.Start();
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="TooltipForm"/> class.
            /// </summary>
            /// <param name="popup">The popup.</param>
            /// <param name="size">The size.</param>
            /// <param name="rect">The rect.</param>
            public TooltipForm(ZeroitTetraToolTip popup, System.Drawing.Size size, Rectangle rect)
            {
                System.Windows.Forms.Padding aPadding = new System.Windows.Forms.Padding();
                mPopup = popup;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                this.DockPadding.All = BORDER_MARGIN;
                aPadding.All = 3;
                if (mPopup._parent != null)
                {
                    Form parentForm = mPopup._parent.FindForm();
                    if (parentForm != null)
                    {
                        parentForm.AddOwnedForm(this);
                    }
                }
                else
                {
                    if (mPopup._control != null)
                    {
                        Form parentForm = mPopup._control.FindForm();
                        if (parentForm != null)
                        {
                            parentForm.AddOwnedForm(this);
                        }
                    }
                }
                this.Padding = aPadding;
                if (mPopup._showShadow)
                {
                    size.Width = size.Width + 10;
                    size.Height = size.Height + 10;
                }
                else
                {
                    size.Width = size.Width + 6;
                    size.Height = size.Height + 6;
                }
                this.MaximumSize = size;
                this.MinimumSize = size;
                bgBitmap = new Bitmap(size.Width, size.Height);
                tBitmap = new Bitmap(size.Width, size.Height);
                ReLocate(rect);
                // Initialize the animation
                mProgress = 0;
                Rectangle aRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                _path = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                this.Location = mNormalPos;
                _timer = new Timer();
                _tmrClose = new Timer();
                _tmrClose.Interval = mPopup._autoClose;
                _tmrClose.Tick += AutoClosing;
                drawBitmap();
                if (mPopup._animationSpeed > 0)
                {
                    _alpha = 0;
                    // I always aim 25 images per seconds.. seems to be a good value
                    // it looks smooth enough on fast computers and do not drain slower one
                    _timer.Interval = mPopup._animationSpeed;
                    mTimerStarted = DateTime.Now;
                    _timer.Tick += Showing;
                    _timer.Start();
                    Showing(null, null);
                }
                else
                {
                    setBitmap(bgBitmap);
                }
                //If mPopup.mDialog Then
                //    ShowDialog()
                //Else
                //    Show()
                //End If
                ShowWindow(this.Handle, SW_NOACTIVATE);
                SetWindowPos(this.Handle, HWND_TOPMOST, this.Left, this.Top, this.Width, this.Height, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                if (mPopup._enableAutoClose)
                {
                    _tmrClose.Start();
                }
            }
            /// <summary>
            /// Draws the transparent bitmap.
            /// </summary>
            private void drawTransparentBitmap()
            {
                Graphics g = Graphics.FromImage(tBitmap);
                int y = 0;
                int x = 0;
                Color aColor = new Color();
                Color tColor = new Color();
                g.Clear(Color.Transparent);
                g.Dispose();
                y = 0;
                while (y < bgBitmap.Height)
                {
                    x = 0;
                    while (x < bgBitmap.Width)
                    {
                        aColor = bgBitmap.GetPixel(x, y);
                        tColor = Color.FromArgb(_alpha * (int)(aColor.A / 100.0), aColor.R, aColor.G, aColor.B);
                        tBitmap.SetPixel(x, y, tColor);
                        x = x + 1;
                    }
                    y = y + 1;
                }
            }
            /// <summary>
            /// Draws the background.
            /// </summary>
            /// <param name="g">The g.</param>
            private void drawBackground(Graphics g)
            {
                if (!mPopup._ownerDrawBackground)
                {
                    if (mPopup._showShadow)
                    {
                        System.Drawing.Drawing2D.LinearGradientBrush bgBrush = null;
                        GraphicsPath aPath = null;
                        Rectangle aRect = new Rectangle(0, 0, this.Width - 4, this.Height - 4);
                        Rectangle rectShadow = new Rectangle(4, 4, this.Width - 4, this.Height - 4);
                        GraphicsPath pathShadow = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(rectShadow, 4, 4, 4, 4);
                        PathGradientBrush shadowBrush = new PathGradientBrush(pathShadow);
                        Color[] sColor = new Color[4];
                        float[] sPos = new float[4];
                        ColorBlend sBlend = new ColorBlend();
                        sColor[0] = Color.FromArgb(0, 0, 0, 0);
                        sColor[1] = Color.FromArgb(16, 0, 0, 0);
                        sColor[2] = Color.FromArgb(32, 0, 0, 0);
                        sColor[3] = Color.FromArgb(128, 0, 0, 0);
                        if (rectShadow.Width > rectShadow.Height)
                        {
                            sPos[0] = 0.0F;
                            sPos[1] = 4 / (float)rectShadow.Width;
                            sPos[2] = 8 / (float)rectShadow.Width;
                            sPos[3] = 1.0F;
                        }
                        else
                        {
                            if (rectShadow.Width < rectShadow.Height)
                            {
                                sPos[0] = 0.0F;
                                sPos[1] = 4 / (float)rectShadow.Height;
                                sPos[2] = 8 / (float)rectShadow.Height;
                                sPos[3] = 1.0F;
                            }
                            else
                            {
                                sPos[0] = 0.0F;
                                sPos[1] = 4 / (float)rectShadow.Width;
                                sPos[2] = 8 / (float)rectShadow.Width;
                                sPos[3] = 1.0F;
                            }
                        }
                        sBlend.Colors = sColor;
                        sBlend.Positions = sPos;
                        shadowBrush.InterpolationColors = sBlend;
                        if (rectShadow.Width > rectShadow.Height)
                        {
                            shadowBrush.CenterPoint = new Point(rectShadow.X + (int)(rectShadow.Width / 2.0), rectShadow.Bottom - (int)(rectShadow.Width / 2.0));
                        }
                        else
                        {
                            if (rectShadow.Width == rectShadow.Height)
                            {
                                shadowBrush.CenterPoint = new Point(rectShadow.X + (int)(rectShadow.Width / 2.0), rectShadow.Y + (int)(rectShadow.Height / 2.0));
                            }
                            else
                            {
                                shadowBrush.CenterPoint = new Point(rectShadow.Right - (int)(rectShadow.Height / 2.0), rectShadow.Y + (int)(rectShadow.Height / 2.0));
                            }
                        }
                        aPath = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                        bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(aRect, Color.FromArgb(255, 255, 255), Color.FromArgb(201, 217, 239), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);
                        g.FillPath(shadowBrush, pathShadow);
                        g.FillPath(bgBrush, aPath);
                        g.DrawPath(new Pen(Color.FromArgb(118, 118, 118)), aPath);
                        bgBrush.Dispose();
                        aPath.Dispose();
                        pathShadow.Dispose();
                        shadowBrush.Dispose();
                    }
                    else
                    {
                        System.Drawing.Drawing2D.LinearGradientBrush bgBrush = null;
                        GraphicsPath aPath = null;
                        Rectangle aRect = new Rectangle(0, 0, this.Width, this.Height);
                        aPath = Zeroit.Framework.Tooltip.ControlRenderer.Drawing.roundedRectangle(aRect, 2, 2, 2, 2);
                        bgBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, this.Width, this.Height), Color.FromArgb(255, 255, 255), Color.FromArgb(201, 217, 239), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);
                        g.FillPath(bgBrush, aPath);
                        g.DrawPath(new Pen(Color.FromArgb(118, 118, 118)), aPath);
                        bgBrush.Dispose();
                        aPath.Dispose();
                    }
                }
                else
                {
                    g.Clear(Color.Transparent);
                    mPopup.invokeDrawBackground(g, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }
            }
            /// <summary>
            /// Draws the bitmap.
            /// </summary>
            private void drawBitmap()
            {
                Graphics g = Graphics.FromImage(bgBitmap);
                Rectangle rect = new Rectangle();
                drawBackground(g);
                if (!mPopup.OwnerDrawBackground)
                {
                    if (mPopup._showShadow)
                    {
                        rect.X = 3;
                        rect.Y = 3;
                        rect.Width = this.Width - 10;
                        rect.Height = this.Height - 10;
                    }
                    else
                    {
                        rect.X = 3;
                        rect.Y = 3;
                        rect.Width = this.Width - 6;
                        rect.Height = this.Height - 6;
                    }
                }
                else
                {
                    rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                }
                mPopup.invokeDraw(g, rect);
                g.Dispose();
            }
            /// <summary>
            /// Sets the bitmap.
            /// </summary>
            /// <param name="aBmp">a BMP.</param>
            private void setBitmap(Bitmap aBmp)
            {
                IntPtr screenDC = GetDC(IntPtr.Zero);
                IntPtr memDC = CreateCompatibleDC(screenDC);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;
                try
                {
                    hBitmap = aBmp.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = SelectObject(memDC, hBitmap);

                    Size size = new Size(aBmp.Width, aBmp.Height);
                    Point pointSource = new Point(0, 0);
                    Point topPos = new Point(this.Left, this.Top);
                    BLENDFUNCTION blend = new BLENDFUNCTION();
                    blend.BlendOp = Convert.ToByte(AC_SRC_OVER);
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = 255;
                    blend.AlphaFormat = Convert.ToByte(AC_SRC_ALPHA);
                    UpdateLayeredWindow(this.Handle, screenDC, ref topPos, ref size, memDC, ref pointSource, 0, ref blend, ULW_ALPHA);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    ReleaseDC(IntPtr.Zero, screenDC);
                    if (hBitmap != IntPtr.Zero)
                    {
                        SelectObject(memDC, oldBitmap);
                        DeleteObject(hBitmap);
                    }
                    DeleteDC(memDC);
                }
            }
            /// <summary>
            /// Gets the create parameters.
            /// </summary>
            /// <value>The create parameters.</value>
            protected override System.Windows.Forms.CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYERED;
                    return cp;
                }
            }
            /// <summary>
            /// Disposes of the resources (other than memory) used by the <see cref="T:System.Windows.Forms.Form" />.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_tmrClose != null)
                    {
                        _tmrClose.Dispose();
                    }
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }
                    if (bgBitmap != null)
                    {
                        bgBitmap.Dispose();
                    }
                    if (tBitmap != null)
                    {
                        tBitmap.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
            /// <summary>
            /// Res the locate.
            /// </summary>
            private void ReLocate()
            {
                int rW = 0;
                int rH = 0;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                Cursor mCursor = mPopup._control.Cursor;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = System.Windows.Forms.Control.MousePosition;
                mNormalPos.X = mNormalPos.X + mCursor.Size.Width;
                mNormalPos.Y = mNormalPos.Y + mCursor.Size.Height;
                if (mNormalPos.X + rW > workingArea.Width)
                {
                    mNormalPos.X = mNormalPos.X - rW;
                }
                if (mNormalPos.Y + rH > workingArea.Height)
                {
                    mNormalPos.Y = mNormalPos.Y - (rH + mCursor.Size.Height);
                }
            }
            /// <summary>
            /// Res the locate.
            /// </summary>
            /// <param name="location">The location.</param>
            private void ReLocate(Point location)
            {
                int rW = 0;
                int rH = 0;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = mPopup._control.PointToScreen(location);
                if (mNormalPos.X + rW > workingArea.Width)
                {
                    mNormalPos.X = mNormalPos.X - rW;
                }
                if (mNormalPos.Y + rH > workingArea.Height)
                {
                    mNormalPos.Y = mNormalPos.Y - rH;
                }
            }
            /// <summary>
            /// Res the locate.
            /// </summary>
            /// <param name="rect">The rect.</param>
            private void ReLocate(Rectangle rect)
            {
                int rW = 0;
                int rH = 0;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                Point askedLoc = new Point();
                askedLoc.X = rect.X;
                askedLoc.Y = rect.Bottom + 5;
                rW = this.Width;
                rH = this.Height;
                mNormalPos = mPopup._control.PointToScreen(askedLoc);
                if (mNormalPos.X + rW > workingArea.Width)
                {
                    mNormalPos.X = mNormalPos.X - (rW - rect.Width);
                }
                if (mNormalPos.Y + rH > workingArea.Height)
                {
                    mNormalPos.Y = mNormalPos.Y - (rH + rect.Height + 10);
                }
            }
            /// <summary>
            /// Showings the specified sender.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
            private void Showing(object sender, EventArgs e)
            {
                if (!_closing)
                {
                    if (_alpha == 100)
                    {
                        _timer.Stop();
                    }
                    else
                    {
                        try
                        {
                            _alpha = _alpha + 10;
                            drawTransparentBitmap();
                            setBitmap(tBitmap);
                        }
                        catch (Exception ex)
                        {
                            _timer.Stop();
                        }
                    }
                }
                else
                {
                    if (_alpha == 0)
                    {
                        _timer.Stop();
                        _timer.Tick -= Showing;
                        invokeClose();
                    }
                    else
                    {
                        try
                        {
                            _alpha = _alpha - 10;
                            drawTransparentBitmap();
                            setBitmap(tBitmap);
                        }
                        catch (Exception ex)
                        {
                            _timer.Stop();
                        }
                    }
                }
            }
            /// <summary>
            /// Does the close.
            /// </summary>
            internal void DoClose()
            {
                if (mPopup._animationSpeed > 0)
                {
                    _closing = true;
                    _timer.Start();
                }
                else
                {
                    invokeClose();
                }
            }
            /// <summary>
            /// Invokes the close.
            /// </summary>
            internal void invokeClose()
            {
                try
                {
                }
                finally
                {
                    mPopup._form.Close();
                    mPopup._form = null;
                    if (mPopup._parent != null)
                    {
                        Form parentForm = mPopup._parent.FindForm();
                        if (parentForm != null)
                        {
                            parentForm.RemoveOwnedForm(this);
                        }
                    }
                    else
                    {
                        if (mPopup._control != null)
                        {
                            Form parentForm = mPopup._control.FindForm();
                            if (parentForm != null)
                            {
                                parentForm.RemoveOwnedForm(this);
                            }
                        }
                    }
                    //parentForm.Focus()
                    Close();
                }
            }
            /// <summary>
            /// Automatics the closing.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
            private void AutoClosing(object sender, EventArgs e)
            {
                DoClose();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Invokes the draw.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        private void invokeDraw(Graphics g, Rectangle rect)
        {
            if (_ownerDraw || _ownerDrawBackground)
            {
                DrawEventArgs e = null;
                e = new DrawEventArgs(g, rect);
                if (Draw != null)
                    Draw(this, e);
            }
            else
            {
                string tTitle = GetToolTipTitle(_control);
                string tText = GetToolTip(_control);
                Image tImage = GetToolTipImage(_control);
                Zeroit.Framework.Tooltip.ControlRenderer.ToolTip.drawToolTip(tTitle, tText, tImage, g, rect);
            }
        }
        /// <summary>
        /// Invokes the draw background.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        private void invokeDrawBackground(Graphics g, Rectangle rect)
        {
            DrawEventArgs e = new DrawEventArgs(g, rect);
            if (DrawBackground != null)
                DrawBackground(this, e);
        }
        /// <summary>
        /// Determines whether [has tool tip] [the specified control].
        /// </summary>
        /// <param name="ctrl">The control.</param>
        /// <returns><c>true</c> if [has tool tip] [the specified control]; otherwise, <c>false</c>.</returns>
        private bool hasToolTip(System.Windows.Forms.Control ctrl)
        {
            string tText = GetToolTip(ctrl);
            string tTitle = GetToolTipTitle(ctrl);
            Image tImage = GetToolTipImage(ctrl);
            return Zeroit.Framework.Tooltip.ControlRenderer.ToolTip.containsToolTip(tTitle, tText, tImage);
        }
        // Control's MouseEnter and MouseLeave event handler
        /// <summary>
        /// Controls the mouse enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ctrlMouseEnter(object sender, EventArgs e)
        {
            _control = (System.Windows.Forms.Control)sender;
            //INSTANT C# NOTE: The following VB 'Select Case' included either a non-ordinal switch expression or non-ordinal, range-type, or non-constant 'Case' expressions and was converted to C# 'if-else' logic:
            //		Select Case _location
            //ORIGINAL LINE: Case ToolTipLocation.Auto
            if (_location == ToolTipLocation.Auto)
            {
                Rectangle ctrlRect = new Rectangle(0, 0, _control.Bounds.Width, _control.Bounds.Height);
                show(_control, ctrlRect);
            }
            //ORIGINAL LINE: Case ToolTipLocation.MousePointer
            else if (_location == ToolTipLocation.MousePointer)
            {
                show(_control);
            }
            //ORIGINAL LINE: Case ToolTipLocation.CustomClient
            else if (_location == ToolTipLocation.CustomClient)
            {
                show(_control, _customLocation);
            }
            //ORIGINAL LINE: Case ToolTipLocation.CustomScreen
            else if (_location == ToolTipLocation.CustomScreen)
            {
                Point clientLocation = _control.PointToClient(_customLocation);
                show(_control, clientLocation);
            }
        }
        /// <summary>
        /// Controls the mouse leave.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ctrlMouseLeave(object sender, EventArgs e)
        {
            if ((System.Windows.Forms.Control)sender == _control)
            {
                _control = null;
                hide();
            }
        }
        /// <summary>
        /// Controls the mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void ctrlMouseDown(object sender, MouseEventArgs e)
        {
            hide();
        }
        // ToolStripItem's MouseEnter and MouseLeave event handler
        /// <summary>
        /// Tsis the mouse enter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsiMouseEnter(object sender, EventArgs e)
        {
            ToolStripItem anItem = (ToolStripItem)sender;
            _control = anItem.GetCurrentParent();
            //INSTANT C# NOTE: The following VB 'Select Case' included either a non-ordinal switch expression or non-ordinal, range-type, or non-constant 'Case' expressions and was converted to C# 'if-else' logic:
            //		Select Case _location
            //ORIGINAL LINE: Case ToolTipLocation.Auto
            if (_location == ToolTipLocation.Auto)
            {
                Rectangle itemRect = new Rectangle(anItem.Bounds.X, 0, anItem.Bounds.Width, _control.Height - 2);
                show(_control, itemRect);
            }
            //ORIGINAL LINE: Case ToolTipLocation.MousePointer
            else if (_location == ToolTipLocation.MousePointer)
            {
                show(_control);
            }
            //ORIGINAL LINE: Case ToolTipLocation.CustomClient
            else if (_location == ToolTipLocation.CustomClient)
            {
                show(_control, _customLocation);
            }
            //ORIGINAL LINE: Case ToolTipLocation.CustomScreen
            else if (_location == ToolTipLocation.CustomScreen)
            {
                Point clientLocation = _control.PointToClient(_customLocation);
                show(_control, clientLocation);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Specifies fade effect period when the tooltip is displayed or hiden, in milliseconds.
        /// </summary>
        /// <value>The animation speed.</value>
        [DefaultValue(20)]
        public int AnimationSpeed
        {
            get
            {
                return _animationSpeed;
            }
            set
            {
                _animationSpeed = value;
            }
        }
        /// <summary>
        /// Show the shadow effect of the tooltip.  This property is ignored when OwnerDrawBackground property is set to true.
        /// </summary>
        /// <value><c>true</c> if [show shadow]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool ShowShadow
        {
            get
            {
                return _showShadow;
            }
            set
            {
                _showShadow = value;
            }
        }
        /// <summary>
        /// Period of time the ZeroitTetraToolTip is displayed, in milliseconds.
        /// </summary>
        /// <value>The automatic close.</value>
        [DefaultValue(3000)]
        public int AutoClose
        {
            get
            {
                return _autoClose;
            }
            set
            {
                _autoClose = value;
            }
        }
        /// <summary>
        /// Automatically close the ZeroitTetraToolTip when the specified time in AutoClose property has been passed.
        /// </summary>
        /// <value><c>true</c> if [enable automatic close]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool EnableAutoClose
        {
            get
            {
                return _enableAutoClose;
            }
            set
            {
                _enableAutoClose = value;
            }
        }
        /// <summary>
        /// ZeroitTetraToolTip surface will be manually drawn by your code.
        /// </summary>
        /// <value><c>true</c> if [owner draw]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool OwnerDraw
        {
            get
            {
                return _ownerDraw;
            }
            set
            {
                _ownerDraw = value;
            }
        }
        /// <summary>
        /// ZeroitTetraToolTip background will be manually drawn by your code.
        /// If this property is set to true, the Draw and Popup event will be raised as well,
        /// and the whole ZeroitTetraToolTip will be drawn by your code.
        /// </summary>
        /// <value><c>true</c> if [owner draw background]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool OwnerDrawBackground
        {
            get
            {
                return _ownerDrawBackground;
            }
            set
            {
                _ownerDrawBackground = value;
            }
        }
        /// <summary>
        /// Determine how the ZeroitTetraToolTip will be located.
        /// </summary>
        /// <value>The location.</value>
        [DefaultValue(typeof(ToolTipLocation), "Auto")]
        public ToolTipLocation Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }
        /// <summary>
        /// Custom location where the ZeroitTetraToolTip will be displayed.
        /// Used when the Location property is set CustomScreen or CustomClient.
        /// </summary>
        /// <value>The custom location.</value>
        [DefaultValue(typeof(Point), "0,0")]
        public Point CustomLocation
        {
            get
            {
                return _customLocation;
            }
            set
            {
                _customLocation = value;
            }
        }
        #endregion

    }
    
}
