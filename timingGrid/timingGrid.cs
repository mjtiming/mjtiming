/*
 * Created by SharpDevelop.
 * User: Murray
 * Date: 5/3/2011
 * Time: 6:01 AM
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace timingGrid
{
	/// <summary>
	/// Modify our up arrow key to always go to the penalty column
	/// </summary>
	public class TimingGrid : DataGridView
	{
		protected override void OnKeyDown( KeyEventArgs e )
		{
			if (e.KeyCode == Keys.Up)
			{
				int currentRow = this.CurrentRow.Index;
				if (currentRow >= 0)
					this.CurrentCell = this.Rows[currentRow].Cells[1];
			}
			if (e.KeyCode == Keys.Down)
			{
				int currentRow = this.CurrentRow.Index;
				if (currentRow >= 0)
					this.CurrentCell = this.Rows[currentRow].Cells[0];
			}
			if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
			{
				int currentRow = this.CurrentRow.Index;
				if (currentRow >= 0)
					this.CurrentCell = this.Rows[currentRow].Cells[0];
			}
			base.OnKeyDown( e );
		}
	}
}