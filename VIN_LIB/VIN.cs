using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace VIN_LIB
{
    public class VIN
    {
        private readonly string allowedSymbols = "0 1 2 3 4 5 6 7 8 9 " +
            "A B C D E F G H J K L M N P R S T U V W X Y Z"
            .Replace(" ", "");
        private readonly string[] zonesAndCountries = new string[]
        {
            "AA-AH","AJ-AN","BA-BE","BF-BK","BL-BR","CA-CE","CF","CK","CL-CR","DA-DE","DF-DK","DL-DR","DS-D0","EA-EE","EF-EK","EL-E0","FA-FE","FF-FK","FL-F0","GA-G0","HA-H0","JA-JT","KA-KE","KF","KK","KL-KR","KS","K0","LA-L0","MA-ME","MF","MK","ML","MR","MS-M0","NF","NK","NL-NR","NT-N0","PA","PE","PF","PK","PL","PR","PS-P0","RA-RE","RF","RK","RL","RR","RS","R0","SA","SM","SN-ST","SU-SZ","S1-S4","TA-TH","TJ-TP","TR-TV","TW-T1","T2-T0","UA-UG","UH-UM","UN-UT","UU-UZ","U1-U4","U5-U7","U8-U0","VA-VE","VF-VR","VS-VW","VX-V2","V3-V5","V6-V0","WA-W0","XA-XE","XF-XK","XL","XR","XS-XW","XX-X2","X3-X0","YA-YE","YF-YK","YL-YR","YS-YW","YX-Y2","Y3-Y5","Y6-Y0","ZA-ZR","ZS-ZW","1A-10","2A-20","3A","3W","3X-37","38-","30","4A-40","5A-50","6A-6W","6X-60","7A-7E","7F-70","8A-8E","8F-8K","8L-8R","8S-8W","8X-82","83-80","9A-9E","9F-9K","9L-9R","9S-9W","9X-92","93-99","90","ZX-Z2","Z3-Z5","Z6-Z0"
        };
        private readonly string[] notZonesAndCountries = new string[]
        {
            "AP-A0","BS-B0","CS-C0","DS-D0","EL-E0","FL-F0","GA-G0","HA-H0","MS-M0","NT-N0","PS-P0","T2-T0","UA-UG","U1-U4","U8-U0","ZS-ZW","6X-60","7F-70","83-80","90"
        };
        public bool CheckVIN(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
            {
                return false;
            }
            if (vin.Any(c => !allowedSymbols.Contains(c)))
            {
                return false;
            }

            string wmi = string.Join("",
                                     vin.Take(3));
            string geographicalZoneAndCountryInZone = string.Join("",
                                                                  wmi.Take(2));
            bool isMatchedAnyCountry = false;
            foreach (string countryMatch in zonesAndCountries.Except(notZonesAndCountries))
            {
                if (geographicalZoneAndCountryInZone[0] == countryMatch[0])
                {
                    if (Regex.IsMatch(
                        geographicalZoneAndCountryInZone[1].ToString(),
                            $"[{countryMatch[1]}-{countryMatch[3]}]"))
                    {
                        isMatchedAnyCountry = true;
                        break;
                    }
                }
            }
            if (!isMatchedAnyCountry)
            {
                return false;
            }
            string vds = string.Join("",
                vin.Skip(3)
                   .Take(6));
            if (vds[5].ToString() != "X"
                && !char.IsDigit(vds[5]))
            {
                return false;
            }
            string vis = string.Join("",
                vin.Reverse()
                    .Take(8)
                    .Reverse());
            if (vis.Reverse()
                   .Take(4)
                   .Any(c =>
                   {
                       return !char.IsDigit(c);
                   }))
            {
                return false;
            }
            if (!Regex.IsMatch(vis[0].ToString(), "[A-Z0-9]"))
            {
                return false;
            }

            char calculatedCharacter = getHashSumCharacter(vin);
            if (calculatedCharacter
                != vin[8])
            {
                return false;
            }
            return true;
        }

        readonly string letters = "A B C D E F G H J K L M N P R S T U V W X Y Z".Replace(" ", "");
        readonly string digits = "1 2 3 4 5 6 7 8 1 2 3 4 5 7 9 2 3 4 5 6 7 8 9".Replace(" ", "");
        readonly string weights = "8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2".Replace(" ", "");
        private char getHashSumCharacter(string vin)
        {
            char[] vinNumbers = vin.ToCharArray();
            int[] replacedVinNumbers = vinNumbers.Select(v =>
            {
                if (char.IsDigit(v))
                {
                    return int.Parse(
                        v.ToString());
                }
                else
                {
                    return int.Parse(
                        digits.ElementAt(letters.IndexOf(v))
                              .ToString());
                }
            }).ToArray();
            int sum = 0;
            for (int i = 0; i < replacedVinNumbers.Length; i++)
            {
                if (i == 8)
                {
                    continue;
                }
                if (i == 7)
                {
                    sum += replacedVinNumbers[i] * 10;
                }
                else
                {
                    sum += replacedVinNumbers[i] * int.Parse(weights[i].ToString());
                }
            }
            int dividedByEleven = (int)Math.Floor((sum / 11.0));
            int multipliedByEleven = dividedByEleven * 11;
            int difference = sum - multipliedByEleven;
            if (difference == 10)
            {
                return 'X';
            }
            return char.Parse(difference.ToString());
        }
    }
}