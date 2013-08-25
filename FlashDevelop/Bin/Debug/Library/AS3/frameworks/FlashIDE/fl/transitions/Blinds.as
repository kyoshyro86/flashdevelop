﻿package fl.transitions
{
	import flash.display.MovieClip;
	import flash.geom.*;

	/**
	 * The Blinds class reveals the movie clip object by using appearing or disappearing rectangles. 
	 */
	public class Blinds extends Transition
	{
		/**
		 * @private
		 */
		protected var _numStrips : uint;
		/**
		 * @private
		 */
		protected var _dimension : uint;
		/**
		 * @private
		 */
		protected var _mask : MovieClip;
		/**
		 * @private
		 */
		protected var _innerMask : MovieClip;

		/**
		 * @private
		 */
		public function get type () : Class;

		/**
		 * @private
		 */
		function Blinds (content:MovieClip, transParams:Object, manager:TransitionManager);
		/**
		 * @private
		 */
		public function start () : void;
		/**
		 * @private
		 */
		public function cleanUp () : void;
		/**
		 * @private
		 */
		protected function _initMask () : void;
		/**
		 * @private
		 */
		protected function _render (p:Number) : void;
	}
}