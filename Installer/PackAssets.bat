@Echo Off

SET PATH=%PATH%;C:\Program Files\7-Zip;

Echo Deleting existing archive...
del "Cos.Assets.rez"

Echo Creatng new archive...
7z.exe a -tzip -mx1 "Cos.Assets.rez" "Assets/." -r

