import ServiceCarousel from "../components/ServiceCarousel";
import PageNav from "../components/PageNav";
import styles from "./Homepage.module.css";
import LocationCarousel from "../components/LocationCarousel";
import HeroSection from "../components/HeroSection";
import ClientSection from "../components/ClientSection";
import AboutUsSection from "../components/AboutUsSection";
import ClientType from "../components/ClientType";
import JoinToday from "../components/JoinToday";
import Footer from "../components/Footer";
export default function Homepage() {
  return (
    <main className={styles.homepage}>
      <PageNav />
      <div className="w-full h-9/12 bg-cover bg-center pt-18 pl-24 relative" style={{ backgroundImage: "url('/imgs/EventLink.png')" }}>
        <div className="absolute inset-0 bg-black opacity-70"></div>
         <div className="relative z-10">
        <HeroSection/>
         </div>
      </div>
      <div className="bg-sky-200 h-50 w-full pt-7">
        <ClientSection/>
      </div>
      <div className="bg-sky-50 h-74 w-full pt-12">
        <AboutUsSection/>
      </div>
      <div className="bg-white h-74 w-full pt-12 bg-cover bg-no-repeat bg-bottom" style={{ backgroundImage: "url('/imgs/Vector4.png')", 
        backgroundSize: "100% 175px"
       }}>
        <ClientType/>
      </div>
      <div className="bg-sky-50 w-full pt-8" style={{ height: "62vw" }}>
        <ServiceCarousel />
      </div>
      <div className="bg-white h-lvw w-full pt-8" style={{ height: "49vw" }}>
        <LocationCarousel/>
      </div>
      <div className=" h-9/12 w-full">
      <JoinToday/>
      </div>

      <div className=" h-56 w-full">
        <Footer/>
      </div>
    </main>
  );
}
