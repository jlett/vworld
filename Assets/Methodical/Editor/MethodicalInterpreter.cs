//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Linkage.Methodical{
	public class MethodicalInterpreter{
		private enum ObjectType {variableType,intType,floatType,stringType,methodType,unknownType};

		private static int safetyIndex = 0;//Using this to prevent freezing the editor if something goes wrong. increment when I advance chunks, reset on new submission
		public static int maxCommandLength = 50;//This is the maximum length of commands interpreted with this class. THis prevents locking up Unity 

		/*!
		 * Interpret a command
		 * */
		public static MethodicalObject MakeChain(MonoBehaviour behaviour, string command){
			
			safetyIndex = 0;//resetting the safety index since we're starting a new interpretation
			MethodicalObject chain = Interpret(behaviour, null,command);

			PrintChain(chain);

			return chain;
		}



		/*!
		 * Recursively loop through command creating a data chain.
		 * */
		private static MethodicalObject Interpret(object context, MethodicalObject chain, string command){
			if (!validate (command)) {
				return new MethodicalErrorObject("",MethodicalErrorObject.ERRORTYPES.UnsupportedUsage);
			}
			
			//Doing some safety stuff to make sure I don't lock up Unity in an infinite loop
			if (safetyIndex > maxCommandLength){
				return null;//TODO Make this return an error to the user.
			}
			safetyIndex++;

			//getting the next chunk
			MethodicalObject chunk = GetNextChunk(context,command);
			if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.equalityType)) {

				//I can safely assume this is the first call of interpret
				chunk.leftEquality = Interpret(context, null, chunk.leftEqualityString);
				
				chunk.rightEquality = Interpret(context, null, chunk.rightEqualityString);
				return chunk;
			}
			else {
				//going to shorten the command so the next chunk starts at the first character
				command = TruncateCommand(chunk, command);
				command = TrimPeriods(command);
				if (chain == null)//starting chain if it isn't initiated, otherwise adding to chain
					chain = chunk;
				else
					chain.AppendToChain(chunk);
				//if there are any chunks left
				if (command != "") {
					return Interpret(context, chain, command);
				}
				return chain; 
			}
			
			
		}



		#region chunk ops
		/*!
		 * This method takes an input string, gets the next chunk, and prepares the string
		 * for retrieval of the next chunk by removing the chunk that was just found
		 * */
		private static MethodicalObject GetNextChunk(object context, string command){
			//Getting next chunk
			MethodicalObject chunk = new MethodicalObject();
			string methodReturn = "";
			string[] equality = new string[0];
			if ((equality = CheckEquality(command)) != null){
				chunk.objectName = "equality";
				chunk.objectType = MethodicalObject.ObjectTypes.equalityType;
				chunk.leftEqualityString = equality[0];
				chunk.leftEqualityString = chunk.leftEqualityString.Trim();
				chunk.rightEqualityString = equality[1];
				chunk.rightEqualityString = chunk.rightEqualityString.Trim();
			}else if ((methodReturn = CheckNumber(command)) != ""){//is the next chunk a number?

				chunk.objectName = methodReturn;//I'll never use this, but it makes printlines readable
				//float or int?
				if (methodReturn.Contains("f")||methodReturn.Contains("F")){
					chunk.objectType = MethodicalObject.ObjectTypes.floatType;
					methodReturn = methodReturn.Replace("f","");
					methodReturn = methodReturn.Replace("F","");
					chunk.value = float.Parse(methodReturn);
				}else if (methodReturn.Contains("d") || methodReturn.Contains("D")){
					chunk.objectType = MethodicalObject.ObjectTypes.DoubleType;
					methodReturn = methodReturn.Replace("d", "");
					methodReturn = methodReturn.Replace("D", "");
					chunk.value =  double.Parse(methodReturn);
				}else{
					chunk.objectType = MethodicalObject.ObjectTypes.intType;
					chunk.value = int.Parse(methodReturn);
				}

				
			}else if ((methodReturn = CheckBool(command)) != ""){//is the next chunk a boolean?

				chunk.objectName = methodReturn;//I'll never use this, but it makes printlines readable
				chunk.value = bool.Parse( methodReturn);
				chunk.objectType = MethodicalObject.ObjectTypes.boolType;
				
			}else if ((methodReturn = CheckString(command)) != ""){//is the next chunk a string?

				chunk.objectName = methodReturn;
				chunk.objectType = MethodicalObject.ObjectTypes.stringType;
				methodReturn = methodReturn.Trim('"');
				chunk.value = methodReturn;
				
			}else if ((methodReturn = CheckArray(context, command)) != ""){//is the next chunk an array?

				chunk.objectName = methodReturn;
				chunk.objectType = MethodicalObject.ObjectTypes.arrayType;
				chunk.arrayDictionaryIndex = Interpret(context,null, GetArrayIndex(command));
				//testIndex = new MethodicalObject();
				//testIndex.value = 0;
				//testIndex.objectName = "0";
				//testIndex.objectType = MethodicalObject.ObjectTypes.arrayType;
				//chunk.arrayDictionaryIndex = new MethodicalObject ();

			}else if ((methodReturn = CheckHashMap(context,command)) != ""){//is the next chunk a hashmap?

				chunk.objectName = methodReturn;
				chunk.objectType = MethodicalObject.ObjectTypes.hashMapType;
				chunk.arrayDictionaryIndex =  Interpret(context,null,GetHashmapIndex(command));
			}else if ((methodReturn = CheckMethod(command)) != ""){//is the next chunk a method? 
				chunk.objectName = methodReturn;
				chunk.objectType = MethodicalObject.ObjectTypes.methodType;
				ProcessMethodChunk(context, chunk, command);// doing some additional processing

			}else if ((methodReturn = CheckVariable(command)) != ""){//is the next chunk a variable?
				chunk.objectName = methodReturn;
				chunk.objectType = MethodicalObject.ObjectTypes.variableType;
			}


			return chunk;
		}

		/*!
		 * Takes in the latest chunk and the command string. Prepares the command string to retrieve the next chunk
		 * */
		public static string TruncateCommand(MethodicalObject chunk,string command){
			//trimming the string!
			int trimLength = 0;
			if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.arrayType)){
				trimLength = command.IndexOf("]") + 1;
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.boolType)){
				trimLength = chunk.value.ToString().Length;
			}
			else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.floatType) || chunk.objectType.Equals(MethodicalObject.ObjectTypes.DoubleType))
			{
				trimLength = chunk.value.ToString().Length+1;
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.hashMapType)){
				trimLength = command.IndexOf("]") + 1;
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.intType)){
				trimLength =  chunk.objectName.Length;
				
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.methodType)){
				string filterString = "(?<=\".*)[\\(\\),](?=.*\")";
				string fixedString = ReplaceMatch(filterString,command," ");
				int startParen = fixedString.IndexOf("(");
				int lastParen = GetClosingParen(startParen,fixedString);
				trimLength = lastParen+1;
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.stringType)){
				trimLength = chunk.value.ToString().Length+2;
			}else if (chunk.objectType.Equals(MethodicalObject.ObjectTypes.variableType)){
				trimLength = chunk.objectName.Length;
			}
			
			command = command.Substring(trimLength,command.Length - trimLength);

			return command;
		}


		#endregion

	//************************** Chunk Checks **************************//
		#region chunk checks	

		/*!
		 * Check if next chunk is an equality and returns each side of equality if so
		 * */
		private static string[] CheckEquality(string input) {
			string[] returnValue = new string[2];
			int equalityIndex = input.IndexOf("=");
			if (equalityIndex >=0 && equalityIndex < input.Length) {
				returnValue[0] = input.Substring(0, equalityIndex);
				returnValue[1] = input.Substring(equalityIndex + 1);
			}
			else {
				returnValue = null;
			}
			return returnValue;
		}


		/*!
		 * Check if next chunk is a number and returns it. returns empty string if not a number
		 * */
		private static string CheckNumber(string input){
			string rString = "^[0-9.]+(f|d)?";
			string capture = "";//matched string
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}

		/*!
		 * Check if next chunk is a boolean and returns it. returns empty string if not a boolean
		 * */
		private static string CheckBool(string input){
			string rString = "^(false|true)";
			string capture = "";
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}

		/*!
		 * Check if next chunk is a string and returns it. returns empty string if not a string
		 * */
		private static string CheckString(string input){
			string rString = "^\".*\"";
			string capture = "";
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
			
		}


		/*!
		 * Check if next chunk is an Array and returns it. returns empty string if not an array
		 * */
		private static string CheckArray(object context, string input){
			string rString = "^[a-z10-9_]+(?=\\[[a-z0-9_]+\\])";
			string capture = "";

			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
				System.Type objectType = MethodicalExecutor.GetTypeFromClass (context, capture);
				if ( objectType.IsArray)
					return capture;
			}
			return "";
		}

		/*!
		 * Check if next chunk is a hashmap and returns it. returns empty string if not a hashmap
		 * */
		private static string CheckHashMap(object context, string input){
			string rString = "^[a-z10-9_]+(?=\\[\"?.*\"?\\])";
			string capture = "";
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;

			}
			return capture;
		}

		/*!
		 * Check if next chunk is a variable and returns it. IT WILL ONLY CHECK FOR PROPER NAMING. 
		 * IT WILL NOT CHECK IF IT IS an int, float, etc...returns empty string if not a variable
		 * */
		private static string CheckVariable(string input){
			string rString = "^[a-z0-9_]+";
			string capture = "";
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}

		/*!
		 * Check if next chunk is a method and returns it. returns empty string if not a method
		 * */
		private static string CheckMethod(string input){
			string rString = "^[a-z0-9_]+(?=\\()";
			string capture = "";
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}
		#endregion


		//************************** Utility methods **************************//

		#region Utility Methods

		private static void PrintChain(MethodicalObject chain){
			string printedLine = "";
			foreach (MethodicalObject mo in chain){
				printedLine = printedLine + "=>" + mo.objectName;
			}
			//Debug.Log(printedLine);
		}

		/*!
		 * Trims the spaces off the string
		 * */
		private static string TrimSpace(string input){
			return input.Trim();
		}

		/*!
		 * Trims the periods off the string
		 * */
		private static string TrimPeriods(string input){
			return input.Trim('.');
		}

		private static string TrimParens(string input){
			if (input.Length > 0){
				string firstCharacter = input.Substring(0,1);
				string lastCharacter = input.Substring(input.Length - 1,1);
				if (firstCharacter == "("){
					input = input.Substring(1);
				}

				if (lastCharacter == ")"){
					input = input.Substring(0,input.Length - 1);
				}

			}
			return input;
		}

		/*!
		 * Retrieves the first array index I can find
		 * */
		private static string GetArrayIndex(string input){
			string rString = "(?<=\\[)[0-9a-z_]+(?=\\])";
			string capture = "";//matched string
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}

		private static string GetHashmapIndex(string input){
			string rString = "(?<=\\[)\"?.+\"?(?=\\])";
			string capture = "";//matched string
			Match match = Regex.Match(input, rString,RegexOptions.IgnoreCase);
			if (match.Success){
				capture = match.Value;
			}
			return capture;
		}

		/*!
		 * Remove remove parts of input that match pattern
		 * */
		private static string ReplaceMatch(string pattern, string input,string replaceWith){
			while (Regex.IsMatch(input,pattern)) {
				input = Regex.Replace(input,pattern,replaceWith);
				
			}
			return input;
		}
		#endregion


		#region method processing
		/*!
		 * Processes a method into a chunk. 
		 * baseObject is the preprocessed chunk. Expects an objectname and objecttype to already be set.
		 * */
		private static MethodicalObject ProcessMethodChunk(object context, MethodicalObject baseObject, string commandString){
			string argumentsString = GetNextArgumentsString(commandString);
			List<string> args = SplitArguments(argumentsString);
			foreach(string argument in args){
				baseObject.arguments.Add( Interpret(context, null,argument) );
			}
			return baseObject;
		}


		/*!
		 * This splits the arguments of the next method into a list<string>
		 * */
		private static List<string> SplitArguments(string argumentString){

			argumentString = TrimParens(argumentString);
			argumentString = TrimSpace(argumentString);

			List<string> arguments = new List<string>();
			//GOing to make sure I actually have arguments. Escape out and return an empty list if I don't
			if (!argumentString.Equals(string.Empty)){

				string argument = argumentString.Substring(0,1);//Making sure I grab the first character
				string filterString = "(?<=\".*)[\\(\\),](?=.*\")";
				string fixedString = ReplaceMatch(filterString,argumentString," ");
				int levelCount = 1;//This keeps track of where I am in the arguments (since arguments might contain their own methods)
				int pointer = 0;//where am I in my string
				while (levelCount > 0 && pointer < argumentString.Length-1){
					pointer++;
					string character = argumentString.Substring(pointer,1);
					string fixedCharacter = fixedString.Substring(pointer,1);
					if (character.Equals("(") && fixedCharacter==character){
						levelCount++;
						argument = argument+character;
					}else if(character.Equals(")")&&fixedCharacter==character){
						levelCount--;
						argument = argument+character;
					}else if (character.Equals(",") && levelCount == 1 && fixedCharacter==character){

						arguments.Add(TrimSpace(argument));
						argument = "";
					}else{
						argument = argument+character;
					}
					
					
				}
				arguments.Add(argument);
			}

			return arguments;
		}

		/*!
		 * This method gets the next set of arguments
		 * */
		private static string GetNextArgumentsString(string commandString){
			string filterString = "(?<=\".*)[\\(\\),](?=.*\")";
			string fixedString = ReplaceMatch(filterString,commandString," ");
			int startParen = fixedString.IndexOf("(");//first paren
			int closingParen = GetClosingParen(startParen,fixedString);
			string returnVal = commandString.Substring(startParen,closingParen-startParen + 1);
			return returnVal;
		}


		/*!
		 * Gets the closing parenthesis for the specified opening paren
		 * */
		private static int GetClosingParen(int openingParen,string commandString){
			if (commandString.Substring(openingParen,1) != "("){//Just covering my bases. I should never hit this in the wild
				Debug.LogError("Not an opening paren!");
			}
			//string openParen2End = commandString.Substring(openingParen,commandString.Length-openingParen);
			int levelCount = 1;//This keeps track of where I am in the arguments (since arguments might contain their own methods)
			int pointer = openingParen;//where am I in my string
			while (levelCount > 0 && pointer < commandString.Length-1){
				pointer++;
				string character = commandString.Substring(pointer,1);
				if (character.Equals("(")){
					levelCount++;
				}else if(character.Equals(")")){
					levelCount--;
				}


			}
			return pointer;
		}
		#endregion

		#region validation
		/*!
		 * Checks the input for banned characters, ignoring stuff in strings
		 * */
		private static bool validate(string input){
			
			//Removing anything within a string so I don't falsly flag a banned character
			input = ReplaceMatch ("\"(\\\"|[^\"])*\"",input,"");

			//Checking for banned characters
			string bannedChars = "<>?;:{}+\\-*\\^";
			string regString = "[" + bannedChars + "]";
			bool hasBannedCharacter = System.Text.RegularExpressions.Regex.IsMatch (input, regString, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			
			//checking that there is only one equality
			int firstEqualityIndex = input.IndexOf("=");
			int lastEqualityIndex = input.LastIndexOf("=");
			bool hasOneEquality = firstEqualityIndex == lastEqualityIndex;

			return !hasBannedCharacter && hasOneEquality;
			
		}
		#endregion
	}
}