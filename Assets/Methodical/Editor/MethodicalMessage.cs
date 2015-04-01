//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;

namespace Linkage.Methodical{
	public class MethodicalMessage {
		public enum MessageTypes{Statement,Return,Log, Error};
		public MessageTypes MessageType = MethodicalMessage.MessageTypes.Log;
		public string message = "";
		

		public MethodicalMessage(string message){
			this.message = message;
		}

		public MethodicalMessage(string message, MessageTypes messageType){
			this.message = message;
			this.MessageType = messageType;
		}

		public static implicit operator MethodicalMessage(string message){
			return new MethodicalMessage (message);
		}

		public static implicit operator string(MethodicalMessage message){
			return message.message;
		}
	}
}