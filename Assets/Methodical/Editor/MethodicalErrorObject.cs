//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;

namespace Linkage.Methodical{
	public class MethodicalErrorObject:MethodicalObject{
		public enum ERRORTYPES
			{
			NoSuchMethod,
			NoSuchField,
			UnsupportedUsage,
			OverrunError,
			DoesNotExist,
			BadArguments,
			OutOfBounds,
			TypeMismatch
			}
		public ERRORTYPES errorType;
		public object cause;//cause of the error
		public MethodicalErrorObject(object cause, ERRORTYPES errorType){
			this.errorType = errorType;
			this.cause = cause;
		}

		public override string ToString ()
		{
			if (errorType.Equals (ERRORTYPES.NoSuchField)) {
				return "Field \"" + cause + "\" does not exist";	
			} else if (errorType.Equals (ERRORTYPES.NoSuchMethod)) {
				return "Method \"" + cause + "\" does not exist";	
			} else if (errorType.Equals (ERRORTYPES.UnsupportedUsage)) {
				return "That operation is unsupported at this time. Stay up-to-date at twitter.com/mark_at_linkage";
			} else if (errorType.Equals (ERRORTYPES.OverrunError)) {
				return "Input String is too long.";//TODO figure out a better way of saying this
			} else if (errorType.Equals (ERRORTYPES.DoesNotExist)) {
				return cause.ToString () + " is not referenced in specified parent class";
			} else if (errorType.Equals (ERRORTYPES.BadArguments)) {
				return "The best overloaded method match for " + cause.ToString () + "() has some invalid arguments";
			} else if (errorType.Equals (ERRORTYPES.OutOfBounds)) {
				return "Index of " + cause + " is out of bounds";
			}
			else if (errorType.Equals(ERRORTYPES.TypeMismatch)) {
				return "Type mismatch error";
			}
			return "Unknown Error";
		}
	}
}