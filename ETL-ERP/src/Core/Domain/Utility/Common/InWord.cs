namespace Domain.Utility.Common
{
    public class InWord
    {
        public InWord()
        {
        }
        public static string ConvertToWordInt(double dblNumber, string taka = "", string paisa = "")//international
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 1000000);
            dblNumber = dblNumber % 1000000;

            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Million";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Million ";
                //strInWord = strInWord + ConverTwoDigit(dblResult) + " Million ";
            }

            //dblResult = Math.Floor(dblNumber / 100000);
            //dblNumber = dblNumber % 100000;
            //if (dblResult > 0)
            //{
            //    strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            //}

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Thousand";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and " + paisa + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = " " + taka + " " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);// + " Only";
            if (taka != "") { tempConvertToWord += " Only"; }

            return tempConvertToWord;
        }
        public static string ConvertToWord(double dblNumber, string taka = "", string paisa = "")
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and " + paisa + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = " " + taka + " " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);// + " Only";
            if (taka != "") { tempConvertToWord += " Only"; }

            return tempConvertToWord;
        }
        public static string ConvertToWordTaka(double dblNumber)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and Paisa" + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = "Taka " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3) + " Only";

            return tempConvertToWord;
        }

        public static string ConvertToWordDollar(double dblNumber)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and cents" + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = "Dollar " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3) + " Only";

            return tempConvertToWord;
        }
        /// <summary>
        /// Convert to text 
        /// </summary>
        /// <param name="dblNumber"></param>
        /// <param name="unit">KG, MT(Metric Ton)</param>
        /// <returns>string text of number with unit added</returns>
        public static string ConvertToWordWeight(double dblNumber, string unit)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            double dblPrecResult = 0;
            double dblPrecLastTwo = 0;
            string strPrecision = "";
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F3}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 3));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            dblNumber = (dblNumber - dblPrecision / 1000.0);
            try
            {
                dblNumber = Convert.ToDouble(strNumber.Split('.')[0]);
            }
            catch (Exception ex) { }

            strInWord = " ";

            if (dblNumber > 999999999.999)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                string partDecimal = "";
                if (unit == "KG")
                {
                    partDecimal = "Grams";
                }
                else if (unit == "MT")
                {
                    partDecimal = "Kilograms";
                }


                dblPrecResult = Math.Floor(dblPrecision / 100);
                dblPrecLastTwo = dblPrecision % 100;
                if (dblPrecResult > 0)
                {
                    strPrecision = ConverTwoDigit(dblPrecResult) + " Hundred ";
                }
                strPrecision = strPrecision + ConverTwoDigit(dblPrecLastTwo);

                strInWord = strInWord + " " + unit + " and " + strPrecision + " " + partDecimal;
                tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);

            }
            else
            {
                // tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);
                tempConvertToWord = strInWord + " " + unit + " Only";
            }

            return tempConvertToWord;
        }
        public static string ConvertToWordWeightInt(double dblNumber, string unit)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            double dblPrecResult = 0;
            double dblPrecLastTwo = 0;
            string strPrecision = "";
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F3}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 3));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            dblNumber = (dblNumber - dblPrecision / 1000.0);
            strInWord = " ";

            if (dblNumber > 999999999.999)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 1000000);
            dblNumber = dblNumber % 1000000;

            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Million";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Million ";
            }

            //dblResult = Math.Floor(dblNumber / 100000);
            //dblNumber = dblNumber % 100000;
            //if (dblResult > 0)
            //{
            //    strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            //}

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Thousand";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                string partDecimal = "";
                if (unit == "KG")
                {
                    partDecimal = "Grams";
                }
                else if (unit == "MT")
                {
                    partDecimal = "Kilograms";
                }


                dblPrecResult = Math.Floor(dblPrecision / 100);
                dblPrecLastTwo = dblPrecision % 100;
                if (dblPrecResult > 0)
                {
                    strPrecision = ConverTwoDigit(dblPrecResult) + " Hundred ";
                }
                strPrecision = strPrecision + ConverTwoDigit(dblPrecLastTwo);

                strInWord = strInWord + " " + unit + " and " + strPrecision + " " + partDecimal;
                tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);

            }
            else
            {
                // tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);
                tempConvertToWord = strInWord + " " + unit + " Only";
            }

            return tempConvertToWord;
        }

        public static string ConvertToWordWeightWithoutOnly(double dblNumber, string unit)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            double dblPrecResult = 0;
            double dblPrecLastTwo = 0;
            string strPrecision = "";
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F3}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 3));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            dblNumber = (dblNumber - dblPrecision / 1000.0);
            strInWord = " ";

            if (dblNumber > 999999999.999)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                string partDecimal = "";
                if (unit == "KG")
                {
                    partDecimal = "Grams";
                }
                else if (unit == "MT")
                {
                    partDecimal = "Kilograms";
                }


                dblPrecResult = Math.Floor(dblPrecision / 100);
                dblPrecLastTwo = dblPrecision % 100;
                if (dblPrecResult > 0)
                {
                    strPrecision = ConverTwoDigit(dblPrecResult) + " Hundred ";
                }
                strPrecision = strPrecision + ConverTwoDigit(dblPrecLastTwo);

                strInWord = strInWord + " " + unit + " and " + strPrecision + " " + partDecimal;
                tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);

            }
            else
            {
                // tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);
                tempConvertToWord = strInWord + " " + unit;
            }

            return tempConvertToWord;
        }

        private static string ConverTwoDigit(double dblTwoDigit)
        {
            double dblFirstDigit = 0;
            double intSecondDigit = 0;
            string[] strArrayFirst = new string[10];
            string[] strArraySecond = new string[10];
            string[] strArrayThird = new string[10];

            strArrayFirst[1] = " One";
            strArrayFirst[2] = " Two";
            strArrayFirst[3] = " Three";
            strArrayFirst[4] = " Four";
            strArrayFirst[5] = " Five";
            strArrayFirst[6] = " Six";
            strArrayFirst[7] = " Seven";
            strArrayFirst[8] = " Eight";
            strArrayFirst[9] = " Nine";

            strArraySecond[1] = " Ten";
            strArraySecond[2] = " Twenty";
            strArraySecond[3] = " Thirty";
            strArraySecond[4] = " Forty";
            strArraySecond[5] = " Fifty";
            strArraySecond[6] = " Sixty";
            strArraySecond[7] = " Seventy";
            strArraySecond[8] = " Eighty";
            strArraySecond[9] = " Ninety";

            strArrayThird[1] = " Eleven";
            strArrayThird[2] = " Twelve";
            strArrayThird[3] = " Thirteen";
            strArrayThird[4] = " Fourteen";
            strArrayThird[5] = " Fifteen";
            strArrayThird[6] = " Sixteen";
            strArrayThird[7] = " Seventeen";
            strArrayThird[8] = " Eighteen";
            strArrayThird[9] = " Nineteen";

            dblFirstDigit = Math.Floor(dblTwoDigit / 10);
            //intSecondDigit = Math.Floor(dblTwoDigit % 10);
            intSecondDigit = FiveToRound(dblTwoDigit % 10);

            if (dblFirstDigit > 0 && intSecondDigit == 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)];
            }

            if (dblFirstDigit == 1 && intSecondDigit > 0)
            {
                return strArrayThird[Convert.ToInt32(intSecondDigit)];
            }

            if (dblFirstDigit == 0 && intSecondDigit > 0)
            {
                return strArrayFirst[Convert.ToInt32(intSecondDigit)]; //sad
            }

            if (dblFirstDigit > 0 & intSecondDigit > 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)] + strArrayFirst[Convert.ToInt32(intSecondDigit)];
            }

            return " ";
        }

        private static int FiveToRound(double dblDigit)
        {
            int intFloor = 0;
            double dblDigit2 = 0;

            intFloor = Convert.ToInt32(Math.Floor(dblDigit));
            dblDigit2 = Convert.ToDouble(intFloor);

            if (dblDigit - dblDigit2 > 0.5)
            {
                return intFloor + 1;
            }
            else
            {
                return intFloor;
            }
        }
        public static string convertEnglistNumberIntoBangla(string englistNumber)
        {
            string banglaNumber = "";
            char[] charArray = englistNumber.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                string character = charArray[i].ToString();
                banglaNumber += getSingleBanglaNumber(character);
            }
            return banglaNumber;
        }
        public static string getSingleBanglaNumber(string character)
        {
            string banglaNumber = "";
            switch (character)
            {
                case "1":
                    banglaNumber = "১";
                    break;
                case "2":
                    banglaNumber = "২";
                    break;
                case "3":
                    banglaNumber = "৩";
                    break;
                case "4":
                    banglaNumber = "৪";
                    break;
                case "5":
                    banglaNumber = "৫";
                    break;
                case "6":
                    banglaNumber = "৬";
                    break;
                case "7":
                    banglaNumber = "৭";
                    break;
                case "8":
                    banglaNumber = "৮";
                    break;
                case "9":
                    banglaNumber = "৯";
                    break;
                case "0":
                    banglaNumber = "০";
                    break;
                case ".":
                    banglaNumber = ".";
                    break;
                default:
                    banglaNumber = character;
                    break;

            }
            return banglaNumber;
        }



        /*Convert Bangla Text*/


        public static string ConvertToWordBangla(double dblNumber)
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigitBangla(dblResult) + " কোটি ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigitBangla(dblResult) + " লক্ষ ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigitBangla(dblResult) + " হাজার ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigitBangla(dblResult) + " শত ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigitBangla(dblResult);
            }

            if (dblPrecision > 0)
            {
                //strInWord = strInWord + " এবং পয়সা" + ConverTwoDigitBangla(dblPrecision % 100);
                strInWord = strInWord + " টাকা এবং " + ConverTwoDigitBangla(dblPrecision % 100) + " পয়সা";
                tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3) + " মাত্র";
            }
            else
            {
                tempConvertToWord = strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3) + " টাকা মাত্র";
            }

            //tempConvertToWord = "টাকা " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3) + " মাত্র";

            return tempConvertToWord;
        }
        private static string ConverTwoDigitBangla(double dblTwoDigit)
        {
            double dblFirstDigit = 0;
            double intSecondDigit = 0;
            string[] strArrayFirst = new string[10];
            string[] strArraySecond = new string[10];
            string[] strArrayThird = new string[10];
            string[] strArrayFourth = new string[10];
            string[] strArrayFiveth = new string[10];
            string[] strArraySixth = new string[10];
            string[] strArraySeven = new string[10];
            string[] strArrayEight = new string[10];
            string[] strArrayNine = new string[10];
            string[] strArrayTen = new string[10];
            string[] strArrayEleven = new string[10];

            strArrayFirst[1] = " এক";
            strArrayFirst[2] = " দুই";
            strArrayFirst[3] = " তিন";
            strArrayFirst[4] = " চার";
            strArrayFirst[5] = " পাঁচ";
            strArrayFirst[6] = " ছয়";
            strArrayFirst[7] = " সাত";
            strArrayFirst[8] = " আট";
            strArrayFirst[9] = " নয়";

            strArraySecond[1] = " দশ";
            strArraySecond[2] = " বিশ";
            strArraySecond[3] = " ত্রিশ";
            strArraySecond[4] = " চল্লিশ";
            strArraySecond[5] = " পঞ্চাশ";
            strArraySecond[6] = " ষাট";
            strArraySecond[7] = " সত্তর";
            strArraySecond[8] = " আশি";
            strArraySecond[9] = " নব্বুই";

            strArrayThird[1] = " এগার";
            strArrayThird[2] = " বার";
            strArrayThird[3] = " তের";
            strArrayThird[4] = " চৌদ্দ";
            strArrayThird[5] = " পনের";
            strArrayThird[6] = " ষোল";
            strArrayThird[7] = " সতের";
            strArrayThird[8] = " আটারো";
            strArrayThird[9] = " উনিশ";

            strArrayFourth[1] = " একুশ";
            strArrayFourth[2] = " বাইশ";
            strArrayFourth[3] = " তেইশ";
            strArrayFourth[4] = " চব্বিশ";
            strArrayFourth[5] = " পঁচিশ";
            strArrayFourth[6] = " ছাব্বিশ";
            strArrayFourth[7] = " সাতাশ";
            strArrayFourth[8] = " আটাশ";
            strArrayFourth[9] = " ঊনত্রিশ";

            strArrayFiveth[1] = " একত্রিশ";
            strArrayFiveth[2] = " বত্রিশ";
            strArrayFiveth[3] = " তেত্রিশ";
            strArrayFiveth[4] = " চৌত্রিশ";
            strArrayFiveth[5] = " পঁয়ত্রিশ";
            strArrayFiveth[6] = " ছয়ত্রিশ";
            strArrayFiveth[7] = " সাইত্রিশ";
            strArrayFiveth[8] = " আটত্রিশ";
            strArrayFiveth[9] = " ঊনচল্লিশ";

            strArraySixth[1] = " একচল্লিশ";
            strArraySixth[2] = " বিয়াল্লিশ";
            strArraySixth[3] = " তেতাল্লিশ";
            strArraySixth[4] = " চুয়াল্লিশ";
            strArraySixth[5] = " পঁয়তাল্লিশ";
            strArraySixth[6] = " ছেচল্লিশ";
            strArraySixth[7] = " সাতচল্লিশ";
            strArraySixth[8] = " আটচল্লিশ";
            strArraySixth[9] = " ঊনপঞ্চাশ";

            strArraySeven[1] = " একান্ন";
            strArraySeven[2] = " বাহান্ন";
            strArraySeven[3] = " তেপ্পান্ন";
            strArraySeven[4] = " চুয়ান্ন";
            strArraySeven[5] = " পঞ্চান্ন";
            strArraySeven[6] = " ছাপ্পান্ন";
            strArraySeven[7] = " সাতান্ন";
            strArraySeven[8] = " আটান্ন";
            strArraySeven[9] = " ঊনষাট";

            strArrayEight[1] = " একষট্টি";
            strArrayEight[2] = " বাষট্টি";
            strArrayEight[3] = " তেষট্টি";
            strArrayEight[4] = " চৌষট্টি";
            strArrayEight[5] = " পঁয়ষট্টি";
            strArrayEight[6] = " ছেষট্টি";
            strArrayEight[7] = " সাতষট্টি";
            strArrayEight[8] = " আটষট্টি";
            strArrayEight[9] = " ঊনসত্তুর";

            strArrayNine[1] = " একাত্তর";
            strArrayNine[2] = " বাহাত্তর";
            strArrayNine[3] = " তেহাত্তর";
            strArrayNine[4] = " চুয়াত্তর";
            strArrayNine[5] = " পচাত্তর";
            strArrayNine[6] = " ছিয়াত্তর";
            strArrayNine[7] = " সাতাত্তর";
            strArrayNine[8] = " আটাত্তর";
            strArrayNine[9] = " ঊনআশি";

            strArrayTen[1] = " একাশি";
            strArrayTen[2] = " বিরাশি";
            strArrayTen[3] = " তিরাশি";
            strArrayTen[4] = " চুরাশি";
            strArrayTen[5] = " পঁচাশি";
            strArrayTen[6] = " ছিয়াশি";
            strArrayTen[7] = " সাতাশি";
            strArrayTen[8] = " আটাশি";
            strArrayTen[9] = " ঊনানব্বুই";

            strArrayEleven[1] = " একানব্বই";
            strArrayEleven[2] = " বিরানব্বই";
            strArrayEleven[3] = " তিরানব্বই";
            strArrayEleven[4] = " চুরানব্বই";
            strArrayEleven[5] = " পঁচানব্বই";
            strArrayEleven[6] = " ছিয়ানব্বই";
            strArrayEleven[7] = " সাতানব্বই";
            strArrayEleven[8] = " আটানব্বই";
            strArrayEleven[9] = " নিরানব্বই";


            dblFirstDigit = Math.Floor(dblTwoDigit / 10);
            //intSecondDigit = Math.Floor(dblTwoDigit % 10);
            intSecondDigit = FiveToRound(dblTwoDigit % 10);

            if (dblFirstDigit > 0 && intSecondDigit == 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)];
            }
            //11-19
            if (dblFirstDigit == 1 && intSecondDigit > 0)
            {
                return strArrayThird[Convert.ToInt32(intSecondDigit)];
            }
            //21-29
            if (dblFirstDigit == 2 && intSecondDigit > 0)
            {
                return strArrayFourth[Convert.ToInt32(intSecondDigit)];
            }
            //31-39
            if (dblFirstDigit == 3 && intSecondDigit > 0)
            {
                return strArrayFiveth[Convert.ToInt32(intSecondDigit)];
            }
            //41-49
            if (dblFirstDigit == 4 && intSecondDigit > 0)
            {
                return strArraySixth[Convert.ToInt32(intSecondDigit)];
            }
            //51-59
            if (dblFirstDigit == 5 && intSecondDigit > 0)
            {
                return strArraySeven[Convert.ToInt32(intSecondDigit)];
            }
            //61-69
            if (dblFirstDigit == 6 && intSecondDigit > 0)
            {
                return strArrayEight[Convert.ToInt32(intSecondDigit)];
            }
            //71-79
            if (dblFirstDigit == 7 && intSecondDigit > 0)
            {
                return strArrayNine[Convert.ToInt32(intSecondDigit)];
            }
            //81-89
            if (dblFirstDigit == 8 && intSecondDigit > 0)
            {
                return strArrayTen[Convert.ToInt32(intSecondDigit)];
            }
            //91-99
            if (dblFirstDigit == 9 && intSecondDigit > 0)
            {
                return strArrayEleven[Convert.ToInt32(intSecondDigit)];
            }

            if (dblFirstDigit == 0 && intSecondDigit > 0)
            {
                return strArrayFirst[Convert.ToInt32(intSecondDigit)]; //sad
            }

            if (dblFirstDigit > 0 & intSecondDigit > 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)] + strArrayFirst[Convert.ToInt32(intSecondDigit)];
            }

            return " ";
        }



        public static string ConvertEnglishToBanglaNumber(int EngNumber)
        {
            string banglaNumber = "";
            switch (EngNumber)
            {
                case 1:
                    banglaNumber = "১";
                    break;
                case 2:
                    banglaNumber = "২";
                    break;
                case 3:
                    banglaNumber = "৩";
                    break;
                case 4:
                    banglaNumber = "৪";
                    break;
                case 5:
                    banglaNumber = "৫";
                    break;
                case 6:
                    banglaNumber = "৬";
                    break;
                case 7:
                    banglaNumber = "৭";
                    break;
                case 8:
                    banglaNumber = "৮";
                    break;
                case 9:
                    banglaNumber = "৯ ";
                    break;
                case 10:
                    banglaNumber = "১০";
                    break;
                case 11:
                    banglaNumber = "১১";
                    break;
                case 12:
                    banglaNumber = "১২";
                    break;
                case 13:
                    banglaNumber = "১৩";
                    break;
                case 14:
                    banglaNumber = "১৪";
                    break;
                case 15:
                    banglaNumber = "১৫";
                    break;
                case 16:
                    banglaNumber = "১৬";
                    break;
                case 17:
                    banglaNumber = "১৭";
                    break;
                case 18:
                    banglaNumber = "১৮";
                    break;
                case 19:
                    banglaNumber = "১৯";
                    break;
                case 20:
                    banglaNumber = "২০";
                    break;
                case 21:
                    banglaNumber = "২১";
                    break;
                case 22:
                    banglaNumber = "২২";
                    break;
                case 23:
                    banglaNumber = "২৩";
                    break;
                case 24:
                    banglaNumber = "২৪";
                    break;
                case 25:
                    banglaNumber = "২৫";
                    break;
                case 26:
                    banglaNumber = "২৬";
                    break;
                case 27:
                    banglaNumber = "২৭";
                    break;
                case 28:
                    banglaNumber = "২৮";
                    break;
                case 29:
                    banglaNumber = "২৯";
                    break;
                case 30:
                    banglaNumber = "৩০";
                    break;
                default:
                    banglaNumber = "";
                    break;

            }
            return banglaNumber;
        }
        public static string ConvertEnglishToBanglaMonth(string EnglishMonth)
        {
            string banglaMonth = "";
            switch (EnglishMonth)
            {
                case "January":
                    banglaMonth = "জানুয়ারী";
                    break;
                case "February":
                    banglaMonth = "ফেব্রুয়ারি";
                    break;
                case "March":
                    banglaMonth = "মার্চ";
                    break;
                case "April":
                    banglaMonth = "এপ্রিল";
                    break;
                case "May":
                    banglaMonth = "মে";
                    break;
                case "June":
                    banglaMonth = "জুন";
                    break;
                case "July":
                    banglaMonth = "জুলাই";
                    break;
                case "August":
                    banglaMonth = "অগাস্ট";
                    break;
                case "September":
                    banglaMonth = "সেপ্টেম্বর";
                    break;
                case "October":
                    banglaMonth = "অক্টোবর";
                    break;
                case "November":
                    banglaMonth = "নভেম্বর";
                    break;
                case "December":
                    banglaMonth = "ডিসেম্বর";
                    break;
                default:
                    banglaMonth = "";
                    break;

            }
            return banglaMonth;
        }
    }
}
