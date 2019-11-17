﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Microsoft.IoT.Lightning.Providers;

namespace UltraborgLib
{
    public class Ultraborg
    {
        private I2cDevice ubI2C { get; set; }
        const int I2C_MAX_LEN = 4;
        const double USM_US_TO_MM = 0.171500;
        const byte I2C_ID_SERVO_USM = 0x36;

        const int COMMAND_GET_TIME_USM1 = 1;     // Get the time measured by ultrasonic #1 in us (0 for no detection)
        const int COMMAND_GET_TIME_USM2 = 2;    // Get the time measured by ultrasonic #2 in us (0 for no detection)
         const int COMMAND_GET_TIME_USM3 = 3;    // Get the time measured by ultrasonic #3 in us (0 for no detection)
        const int COMMAND_GET_TIME_USM4 = 4;     // Get the time measured by ultrasonic #4 in us (0 for no detection)

         const int COMMAND_SET_PWM1 = 5;  // Set the PWM duty cycle for drive #1 (16 bit)
         const int COMMAND_GET_PWM1 = 6;     // Get the PWM duty cycle for drive #1 (16 bit)
         const int COMMAND_SET_PWM2 = 7;   // Set the PWM duty cycle for drive #2 (16 bit)
         const int COMMAND_GET_PWM2 = 8;    // Get the PWM duty cycle for drive #2 (16 bit)
         const int COMMAND_SET_PWM3 = 9;     // Set the PWM duty cycle for drive #3 (16 bit)
         const int COMMAND_GET_PWM3 = 10;    // Get the PWM duty cycle for drive #3 (16 bit)
         const int COMMAND_SET_PWM4 = 11;    // Set the PWM duty cycle for drive #4 (16 bit)
         const int COMMAND_GET_PWM4 = 12;    // Get the PWM duty cycle for drive #4 (16 bit)
         const int COMMAND_CALIBRATE_PWM1 = 13;   // Set the PWM duty cycle for drive #1 (16 bit, ignores limit checks)
         const int COMMAND_CALIBRATE_PWM2 = 14;  // Set the PWM duty cycle for drive #2 (16 bit, ignores limit checks)
         const int COMMAND_CALIBRATE_PWM3 = 15;  // Set the PWM duty cycle for drive #3 (16 bit, ignores limit checks)
         const int COMMAND_CALIBRATE_PWM4 = 16;  // Set the PWM duty cycle for drive #4 (16 bit, ignores limit checks)
         const int COMMAND_GET_PWM_MIN_1 = 17;   // Get the minimum allowed PWM duty cycle for drive #1
         const int COMMAND_GET_PWM_MAX_1 = 18;   // Get the maximum allowed PWM duty cycle for drive #1
         const int COMMAND_GET_PWM_BOOT_1 = 19;  // Get the startup PWM duty cycle for drive #1
         const int COMMAND_GET_PWM_MIN_2 = 20;   // Get the minimum allowed PWM duty cycle for drive #2
         const int COMMAND_GET_PWM_MAX_2 = 21;   // Get the maximum allowed PWM duty cycle for drive #2
         const int COMMAND_GET_PWM_BOOT_2 = 22;  // Get the startup PWM duty cycle for drive #2
         const int COMMAND_GET_PWM_MIN_3 = 23;   // Get the minimum allowed PWM duty cycle for drive #3
         const int COMMAND_GET_PWM_MAX_3 = 24;   // Get the maximum allowed PWM duty cycle for drive #3
        const int COMMAND_GET_PWM_BOOT_3 = 25;   // Get the startup PWM duty cycle for drive #3
         const int COMMAND_GET_PWM_MIN_4 = 26;   // Get the minimum allowed PWM duty cycle for drive #4
         const int COMMAND_GET_PWM_MAX_4 = 27;   // Get the maximum allowed PWM duty cycle for drive #4
        const int COMMAND_GET_PWM_BOOT_4 = 28;   // Get the startup PWM duty cycle for drive #4
         const int COMMAND_SET_PWM_MIN_1 = 29;   // Set the minimum allowed PWM duty cycle for drive #1
         const int COMMAND_SET_PWM_MAX_1 = 30;   // Set the maximum allowed PWM duty cycle for drive #1
        const int COMMAND_SET_PWM_BOOT_1 = 31;   // Set the startup PWM duty cycle for drive #1
         const int COMMAND_SET_PWM_MIN_2 = 32;   // Set the minimum allowed PWM duty cycle for drive #2
         const int COMMAND_SET_PWM_MAX_2 = 33;   // Set the maximum allowed PWM duty cycle for drive #2
         const int COMMAND_SET_PWM_BOOT_2 = 34;  // Set the startup PWM duty cycle for drive #2
         const int COMMAND_SET_PWM_MIN_3 = 35;   // Set the minimum allowed PWM duty cycle for drive #3
         const int COMMAND_SET_PWM_MAX_3 = 36;   // Set the maximum allowed PWM duty cycle for drive #3
         const int COMMAND_SET_PWM_BOOT_3 = 37;  // Set the startup PWM duty cycle for drive #3
         const int COMMAND_SET_PWM_MIN_4 = 38;   // Set the minimum allowed PWM duty cycle for drive #4
         const int COMMAND_SET_PWM_MAX_4 = 39;   // Set the maximum allowed PWM duty cycle for drive #4
         const int COMMAND_SET_PWM_BOOT_4 = 40;  // Set the startup PWM duty cycle for drive #4

        const int COMMAND_GET_FILTER_USM1 = 41;    // Get the filtered time measured by ultrasonic #1 in us (0 for no detection)
        const int COMMAND_GET_FILTER_USM2 = 42;    // Get the filtered time measured by ultrasonic #2 in us (0 for no detection)
        const int COMMAND_GET_FILTER_USM3 = 43;    // Get the filtered time measured by ultrasonic #3 in us (0 for no detection)
        const int COMMAND_GET_FILTER_USM4 = 44;    //Get the filtered time measured by ultrasonic #4 in us (0 for no detection)


        public Ultraborg()
        {
        }

        public async Task<bool> Init(bool useLightning)
        {
            // Set the I2C address and speed
            var settings = new I2cConnectionSettings(I2C_ID_SERVO_USM);
            settings.BusSpeed = I2cBusSpeed.StandardMode;

            if (useLightning && LightningProvider.IsLightningEnabled)
            {
                I2cController controller = (await I2cController.GetControllersAsync(LightningI2cProvider.GetI2cProvider()))[0];
                ubI2C = controller.GetDevice(settings);
            }
            else
            {


                // Try to find the UltraBorg on the I2C bus
                string aqs = I2cDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                ubI2C = await I2cDevice.FromIdAsync(dis[0].Id, settings);
              
            }

            return ubI2C != null;

        }



        private void WriteCommand(byte command, int commandValue )
        {
            byte byteValLow = (byte)(commandValue & 0xFF);
            byte byteValHigh = (byte)(commandValue >> 8 & 0xFF);

            byte[] sendBytes = new byte[] { command, byteValHigh, byteValLow };
            ubI2C.Write(sendBytes);
        }

        private int GetIntegerValue(byte commandValue)
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { commandValue };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);

            int minVal = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            return minVal;
        }

        public ServoLimits GetServoLimits(int ServoNo)
        {
            var servoLimits = new ServoLimits();
            switch (ServoNo)
            {
                case 1: 
                        servoLimits.Start = GetServoStart1();
                        servoLimits.Minimum = GetServoMinimum1();
                        servoLimits.Maximum = GetServoMaximum1();
                        break;
                case 2:
                        servoLimits.Start = GetServoStart2();
                        servoLimits.Minimum = GetServoMinimum2();
                        servoLimits.Maximum = GetServoMaximum2();
                        break;
                case 3:
                        servoLimits.Start = GetServoStart3();
                        servoLimits.Minimum = GetServoMinimum3();
                        servoLimits.Maximum = GetServoMaximum3();
                        break;
                case 4:
                    servoLimits.Start = GetServoStart4();
                    servoLimits.Minimum = GetServoMinimum4();
                    servoLimits.Maximum = GetServoMaximum4();
                    break;
            }

            return servoLimits;
        }

        public void CalibrateServoPosition(int servoNo,int pwmLevel)
        {
            switch (servoNo)
            {
                case 1:CalibrateServoPosition1(pwmLevel);break;
                case 2: CalibrateServoPosition2(pwmLevel); break;
                case 3: CalibrateServoPosition3(pwmLevel); break;
                case 4: CalibrateServoPosition4(pwmLevel); break;
            }
        }

        public void SetServoMinimum(int servoNo, int pwmLevel)
        {
            switch (servoNo)
            {
                case 1: SetServoMinimum1(pwmLevel); break;
                case 2: SetServoMinimum2(pwmLevel); break;
                case 3: SetServoMinimum3(pwmLevel); break;
                case 4: SetServoMinimum4(pwmLevel); break;
            }
        }
        public void SetServoMaximum(int servoNo,int pwmLevel)
        {
            switch (servoNo)
            {
                case 1: SetServoMaximum1(pwmLevel); break;
                case 2: SetServoMaximum2(pwmLevel); break;
                case 3: SetServoMaximum3(pwmLevel); break;
                case 4: SetServoMaximum4(pwmLevel); break;
            }
        }
        public void SetServoBoot(int servoNo,int pwmLevel)
        {
            switch (servoNo)
            {
                case 1: SetServoBoot1(pwmLevel); break;
                case 2: SetServoBoot2(pwmLevel); break;
                case 3: SetServoBoot3(pwmLevel); break;
                case 4: SetServoBoot4(pwmLevel); break;
            }
        }




        public void SetServoPosition(int servoNo,double position, int pwmMin, int pwmMax)
        {
            switch (servoNo)
            {
                case 1: SetServoPosition1(position, pwmMin, pwmMax); break;
                case 2: SetServoPosition2(position, pwmMin, pwmMax); break;
                case 3: SetServoPosition3(position, pwmMin, pwmMax); break;
                case 4: SetServoPosition4(position, pwmMin, pwmMax); break;
            }
        }


        public void GetServoPosition(int servoNo)
        {
            switch (servoNo)
            {
                case 1: GetServoPosition1(); break;
                case 2: GetServoPosition2(); break;
                case 3: GetServoPosition3(); break;
                case 4: GetServoPosition4(); break;
            }
        }

        public int GetServoMinimum(int servoNo)
        {
            switch (servoNo)
            {
                case 1: return GetServoMinimum1();
                case 2: return GetServoMinimum2();
                case 3: return GetServoMinimum3();
                case 4: return GetServoMinimum4();
            }
            return -1;
        }

        public int GetServoMaximum(int servoNo)
        {
            switch (servoNo)
            {
                case 1: return GetServoMaximum1();
                case 2: return GetServoMaximum2();
                case 3: return GetServoMaximum3();
                case 4: return GetServoMaximum4();
            }

            return -1;
        }


        public int GetServoStart(int servoNo)
        {
            switch (servoNo)
            {
                case 1: return GetServoStart1(); 
                case 2: return GetServoStart2();
                case 3: return GetServoStart3();
                case 4: return GetServoStart4();
            }

            return -1;
        }

        //servo 1
        public void CalibrateServoPosition1(int pwmLevel)
        {
            WriteCommand(COMMAND_CALIBRATE_PWM1, pwmLevel);
        }

        public void SetServoPosition1(double position, int pwmMin, int pwmMax)
        {
            // Work out the values to send based on the limits
            // We are using the defaults to keep the example simple
            double powerOut = (position + 1.0) / 2.0;
            int pwmDuty = (int)((powerOut * (pwmMax - pwmMin)) + pwmMin);

            WriteCommand(COMMAND_SET_PWM1, pwmDuty);
        }

        public int GetServoPosition1()
        {
            return GetIntegerValue(COMMAND_GET_PWM1);
        }

        public void SetServoMinimum1(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MIN_1, pwmLevel);
        }

        public void SetServoMaximum1(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MAX_1, pwmLevel);
        }

        public void SetServoBoot1(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_BOOT_1, pwmLevel);
        }

        public int GetServoMinimum1()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MIN_1);
        }

        public int GetServoMaximum1()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MAX_1);
        }


        public int GetServoStart1()
        {
            return GetIntegerValue(COMMAND_GET_PWM_BOOT_1);
        }

        //servo2

        public void CalibrateServoPosition2(int pwmLevel)
        {
            WriteCommand(COMMAND_CALIBRATE_PWM2, pwmLevel);
        }

        public void SetServoPosition2(double position, int pwmMin, int pwmMax)
        {
            // Work out the values to send based on the limits
            // We are using the defaults to keep the example simple
            double powerOut = (position + 1.0) / 2.0;
            int pwmDuty = (int)((powerOut * (pwmMax - pwmMin)) + pwmMin);

            WriteCommand(COMMAND_SET_PWM2, pwmDuty);
        }

        public int GetServoPosition2()
        {
            return GetIntegerValue(COMMAND_GET_PWM2);
        }


        public void SetServoMinimum2(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MIN_2, pwmLevel);
        }

        public void SetServoMaximum2(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MAX_2, pwmLevel);
        }

        public void SetServoBoot2(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_BOOT_2, pwmLevel);
        }


        public int GetServoMinimum2()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MIN_2);

        }

        public int GetServoMaximum2()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MAX_2);
        }


        public int GetServoStart2()
        {
            return GetIntegerValue(COMMAND_GET_PWM_BOOT_2);
        }


        // Servo 3

        public void CalibrateServoPosition3(int pwmLevel)
        {
            WriteCommand(COMMAND_CALIBRATE_PWM3, pwmLevel);
        }


        public void SetServoPosition3(double position, int pwmMin, int pwmMax)
        {
            // Work out the values to send based on the limits
            // We are using the defaults to keep the example simple
            double powerOut = (position + 1.0) / 2.0;
            int pwmDuty = (int)((powerOut * (pwmMax - pwmMin)) + pwmMin);

            WriteCommand(COMMAND_SET_PWM3, pwmDuty);
        }

        public int GetServoPosition3()
        {
            return GetIntegerValue(COMMAND_GET_PWM3);
        }

        public void SetServoMinimum3(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MIN_3, pwmLevel);
        }

        public void SetServoMaximum3(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MAX_3, pwmLevel);
        }

        public void SetServoBoot3(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_BOOT_3, pwmLevel);
        }


        public int GetServoMinimum3()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MIN_3);

        }

        public int GetServoMaximum3()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MAX_3);
        }


        public int GetServoStart3()
        {
            return GetIntegerValue(COMMAND_GET_PWM_BOOT_3);
        }


        // Servo 4
        public int GetServoPosition4()
        {
            return GetIntegerValue(COMMAND_GET_PWM4);
        }


        public void CalibrateServoPosition4(int pwmLevel)
        {
            WriteCommand(COMMAND_CALIBRATE_PWM4, pwmLevel);
        }

        public void SetServoPosition4(double position, int pwmMin, int pwmMax)
        {
            // Work out the values to send based on the limits
            // We are using the defaults to keep the example simple
            double powerOut = (position + 1.0) / 2.0;
            int pwmDuty = (int)((powerOut * (pwmMax - pwmMin)) + pwmMin);

            WriteCommand(COMMAND_SET_PWM4, pwmDuty);
        }
        

        public void SetServoMinimum4(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MIN_4, pwmLevel);
        }

        public void SetServoMaximum4(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_MAX_4, pwmLevel);
        }

        public void SetServoBoot4(int pwmLevel)
        {
            WriteCommand(COMMAND_SET_PWM_BOOT_4, pwmLevel);
        }



        public int GetServoMinimum4()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MIN_4);
        }

        public int GetServoMaximum4()
        {
            return GetIntegerValue(COMMAND_GET_PWM_MAX_4);
        }


        public int GetServoStart4()
        {
            return GetIntegerValue(COMMAND_GET_PWM_BOOT_4);
        }

        //sonic sensors 

        public double GetFilteredDistance(int sensorNo)
        {
            switch (sensorNo)
            {
                case 1:return GetFilteredDistance1();
                case 2: return GetFilteredDistance2();
                case 3: return GetFilteredDistance3();
                case 4: return GetFilteredDistance4();
            }

            return -1;
        }

        public double GetDistance(int sensorNo)
        {
            switch (sensorNo)
            {
                case 1: return GetDistance1();
                case 2: return GetDistance2();
                case 3: return GetDistance3();
                case 4: return GetDistance4();
            }

            return -1;
        }

        public double GetFilteredDistance1()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_FILTER_USM1 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public  double GetDistance1()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_TIME_USM1 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetFilteredDistance2()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_FILTER_USM2 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetDistance2()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_TIME_USM2 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetFilteredDistance3()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_FILTER_USM3 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetDistance3()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_TIME_USM3 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetFilteredDistance4()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_FILTER_USM4 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }

        public double GetDistance4()
        {
            byte[] recvBytes = new byte[I2C_MAX_LEN];
            byte[] sendBytes = new byte[] { COMMAND_GET_TIME_USM4 };

            ubI2C.Write(sendBytes);
            ubI2C.Read(recvBytes);
            int time_us = ((int)recvBytes[1] << 8) + (int)recvBytes[2];
            if (time_us == 65535) time_us = 0; // Error value
            return (double)time_us * USM_US_TO_MM;
        }
    }
}
