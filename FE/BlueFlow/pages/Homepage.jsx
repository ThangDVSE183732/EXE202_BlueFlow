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
import WaterEffects from "../components/WaterEffects";
export default function Homepage() {
  return (
    <main className={styles.homepage}>
      <WaterEffects />
      <PageNav />
      <div className="w-full h-9/12 bg-cover bg-center pt-18 pl-24 relative" style={{ backgroundImage: "url('/imgs/EventLink.png')" }}>
        <div className="absolute inset-0 bg-black opacity-70"></div>
         <div className="relative z-10">
        <HeroSection/>
         </div>
      </div>
      <div className="bg-sky-200/80 h-50 w-full pt-7 relative">
        <div className="relative z-10">
          <ClientSection/>
        </div>
      </div>
      <div className="bg-sky-50/80 h-74 w-full pt-12 relative">
        <div className="relative z-10">
          <AboutUsSection/>
        </div>
      </div>
      <div className="bg-white/80 h-74 w-full pt-12 bg-cover bg-no-repeat bg-bottom relative" style={{ backgroundImage: "url('/imgs/Vector4.png')", 
        backgroundSize: "100% 175px"
       }}>
        <div className="relative z-10">
          <ClientType/>
        </div>
      </div>
      <div className="bg-sky-50/80 w-full pt-8 relative" style={{ height: "62vw" }}>
        <div className="relative z-10">
          <ServiceCarousel />
        </div>
      </div>
      <div className="bg-white/80 h-lvw w-full pt-8 relative" style={{ height: "49vw" }}>
        <div className="relative z-10">
          <LocationCarousel/>
        </div>
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
