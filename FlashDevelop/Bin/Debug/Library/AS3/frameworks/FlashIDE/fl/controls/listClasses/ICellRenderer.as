﻿package fl.controls.listClasses
{
	import fl.controls.listClasses.ListData;

	/**
	 * The ICellRenderer interface provides the methods and properties that a cell renderer requires.
	 */
	public interface ICellRenderer
	{
		/**
		 * @private
		 */
		public function set y (y:Number) : void;
		/**
		 * @private
		 */
		public function set x (x:Number) : void;
		/**
		 * Gets or sets the list properties that are applied to the cell--for example,
		 */
		public function get listData () : ListData;
		/**
		 * @private (setter)
		 */
		public function set listData (value:ListData) : void;
		/**
		 * Gets or sets an Object that represents the data that is 
		 */
		public function get data () : Object;
		/**
		 * @private
		 */
		public function set data (value:Object) : void;
		/**
		 * Gets or sets a Boolean value that indicates whether the
		 */
		public function get selected () : Boolean;
		/**
		 * @private (setter)
		 */
		public function set selected (value:Boolean) : void;

		/**
		 * Sets the size of the data according to the pixel values specified by the <code>width</code>
		 */
		public function setSize (width:Number, height:Number) : void;
		/**
		 * Sets the current cell to a specific mouse state.  This method 
		 */
		public function setMouseState (state:String) : void;
	}
}