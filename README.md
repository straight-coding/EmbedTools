# EmbedTools

* Receive debug log broadcasted by device (via.UDP, 在8899端口侦听设备广播出来的日志).
* Simply generate RSA self-signed certificates using BouncyCastle package, and convert to a c source file `lwip_cert.c`. (产生 PEM 格式的自签服务器证书)
* Compress and pack web pages, then convert to a c source file `fs_data.c`. (gzip压缩网页，并转换成c源文件)

# UDP Log Receiver

 ![stack](/udp-log.png)
  
# RSA Certificate Generator

* This certificate tool will generate a c source file named lwip_cert.c, which includs both PEM certificate and private key.
 ![stack](/certificate.png)
  
# Web Pages Compressor

* This tool will compress and pack web pages into a c source file named fs_data.c.
 ![stack](/webpack.png)
