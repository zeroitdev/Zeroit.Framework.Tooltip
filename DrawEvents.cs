// ***********************************************************************
// Assembly         : Zeroit.Framework.Tooltip
// Author           : ZEROIT
// Created          : 12-21-2018
//
// Last Modified By : ZEROIT
// Last Modified On : 12-21-2018
// ***********************************************************************
// <copyright file="DrawEvents.cs" company="Zeroit Dev Technologies">
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
using System.Drawing;

namespace Zeroit.Framework.Tooltip
{

    /// <summary>
    /// Class DrawEventArgs.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DrawEventArgs : EventArgs
    {
        /// <summary>
        /// The g
        /// </summary>
        private System.Drawing.Graphics _g;
        /// <summary>
        /// The rect
        /// </summary>
        private Rectangle _rect;
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawEventArgs"/> class.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        public DrawEventArgs(System.Drawing.Graphics g, System.Drawing.Rectangle rect) : base()
        {
            _g = g;
            _rect = rect;
        }
        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public System.Drawing.Graphics Graphics
        {
            get
            {
                return _g;
            }
        }
        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return _rect;
            }
        }
    }
}
